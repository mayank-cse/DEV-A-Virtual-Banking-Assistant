using DevVirtualBankingAssistant.Models;
using DevVirtualBankingAssistant.Services;
using DevVirtualBankingAssistant.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Newtonsoft.Json;
namespace DevVirtualBankingAssistant.Dialogs.Operations
{
    public class InvoiceDetection : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        CosmosDBClient _cosmosDBClient;
        //private readonly string CheckProductDialogID = "CheckProductDlg";
        StateService _stateService;

        public InvoiceDetection(IConfiguration configuration, CosmosDBClient cosmosDBClient, StateService stateService) : base(nameof(InvoiceDetection))
        {
            Configuration = configuration;
            _cosmosDBClient = cosmosDBClient;
            _stateService = stateService;

            var waterfallSteps = new WaterfallStep[]
            {
                GetInvoiceStepAsync,
                InputInvoiceStepAsync

            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt)));

            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new AddMoreProductsDialog());
            AddDialog(new GetProductIDDialog(_cosmosDBClient));
            //AddDialog(new TextPrompt(CheckProductDialogID, ProductExistsValidation));

            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> GetInvoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("You have selected Invoice analysis"), cancellationToken);
            /*
            var pmoptions = new PromptOptions();
            pmoptions.Prompt = MessageFactory.Text("Please upload the signed cheque");
            return await stepContext.PromptAsync(nameof(AttachmentPrompt), pmoptions, cancellationToken);*/
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please upload the invoice Example - 'https://raw.githubusercontent.com/mayank-cse/DEV-A-Virtual-Banking-Assistant/91595843ece269dfd871fd3ca90a025cde7b572e/sample-invoice.pdf'"),
                //Choices = ChoiceFactory.ToChoices(new List<string> { "Enter Product ID", "View Last Product ID" }),
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> InputInvoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var inv = (string)stepContext.Result;

            if (inv != null && inv.Length > 0)
            {

                string endpoint = "https://dev262.cognitiveservices.azure.com/";
                string key = "db071ae08bab4e57b35a0ef7d2a8c216";
                AzureKeyCredential credential = new AzureKeyCredential(key);
                DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

                //sample invoice document

                Uri invoiceUri = new Uri(inv);

                AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-invoice", invoiceUri);

                AnalyzeResult result = operation.Value;

                for (int i = 0; i < result.Documents.Count; i++)
                {
                    
                    AnalyzedDocument document = result.Documents[i];

                    if (document.Fields.TryGetValue("VendorName", out DocumentField vendorNameField))
                    {
                        if (vendorNameField.FieldType == DocumentFieldType.String)
                        {
                            string vendorName = vendorNameField.Value.AsString();
                            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Vendor Name: '{vendorName}', with confidence {vendorNameField.Confidence}"),cancellationToken);
                        }
                    }

                    if (document.Fields.TryGetValue("CustomerName", out DocumentField customerNameField))
                    {
                        if (customerNameField.FieldType == DocumentFieldType.String)
                        {
                            string customerName = customerNameField.Value.AsString();
                            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Customer Name: '{customerName}', with confidence {customerNameField.Confidence}"),cancellationToken);
                        }
                    }
                    var attachments = new List<Attachment>();
                    var reply = MessageFactory.Attachment(attachments);
                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    string VendorName = vendorNameField.Value.AsString();
                    string CustomerName = customerNameField.Value.AsString();
                    string Amount = "";
                    string Tax = "";
                    string SubTotal = "";
                    string Description = "";
                    if (document.Fields.TryGetValue("Items", out DocumentField itemsField))
                    {
                        if (itemsField.FieldType == DocumentFieldType.List)
                        {
                            foreach (DocumentField itemField in itemsField.Value.AsList())
                            {
                                //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Item:"),cancellationToken);

                                if (itemField.FieldType == DocumentFieldType.Dictionary)
                                {
                                    IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                                    if (itemFields.TryGetValue("Description", out DocumentField itemDescriptionField))
                                    {
                                        if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                        {
                                            string itemDescription = itemDescriptionField.Value.AsString();
                                            Description = $"{itemDescription}";
                                            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}"),cancellationToken);
                                        }
                                    }

                                    if (itemFields.TryGetValue("Amount", out DocumentField itemAmountField))
                                    {
                                        if (itemAmountField.FieldType == DocumentFieldType.Currency)
                                        {
                                            CurrencyValue itemAmount = itemAmountField.Value.AsCurrency();
                                            Amount = $"{itemAmount.Symbol}{itemAmount.Amount}";
                                            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"  Amount: '{itemAmount.Symbol}{itemAmount.Amount}', with confidence {itemAmountField.Confidence}"),cancellationToken);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    if (document.Fields.TryGetValue("SubTotal", out DocumentField subTotalField))
                    {
                        if (subTotalField.FieldType == DocumentFieldType.Currency)
                        {
                            CurrencyValue subTotal = subTotalField.Value.AsCurrency();
                            
                            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sub Total: '{subTotal.Symbol}{subTotal.Amount}', with confidence {subTotalField.Confidence}"),cancellationToken);
                        }
                    }

                    if (document.Fields.TryGetValue("TotalTax", out DocumentField totalTaxField))
                    {
                        if (totalTaxField.FieldType == DocumentFieldType.Currency)
                        {
                            CurrencyValue totalTax = totalTaxField.Value.AsCurrency();
                            Tax = $"{totalTax.Symbol}{totalTax.Amount}";
                            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Total Tax: '{totalTax.Symbol}{totalTax.Amount}', with confidence {totalTaxField.Confidence}"),cancellationToken);
                        }
                    }

                    if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField invoiceTotalField))
                    {
                        if (invoiceTotalField.FieldType == DocumentFieldType.Currency)
                        {
                            CurrencyValue invoiceTotal = invoiceTotalField.Value.AsCurrency();
                            SubTotal = $"{invoiceTotal.Symbol}{invoiceTotal.Amount}";
                            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Invoice Total: '{invoiceTotal.Symbol}{invoiceTotal.Amount}', with confidence {invoiceTotalField.Confidence}"), cancellationToken);
                        }
                    }
                    reply.Attachments.Add(Cards.Cards.GetHeroCardInvoice(VendorName, CustomerName, Amount, Tax, SubTotal, Description).ToAttachment());
                    await stepContext.Context.SendActivityAsync(reply, cancellationToken);
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"This is the invoice of payment done by {CustomerName} to {VendorName} of {SubTotal} paying tax of {Tax} "), cancellationToken);

                    //Typing... Message
                    await stepContext.Context.SendActivitiesAsync(
                    new Activity[] {
                    new Activity { Type = ActivityTypes.Typing },
                    new Activity { Type = "delay", Value= 15000 },
                        MessageFactory.Text("...", "..."),
                            },
                    cancellationToken);
                }


            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Please provide the invoice URL like this - 'Please upload the invoice Example - 'https://raw.githubusercontent.com/mayank-cse/DEV-A-Virtual-Banking-Assistant/91595843ece269dfd871fd3ca90a025cde7b572e/sample-invoice.pdf''"), cancellationToken);
            
            }
            
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        
    }
}
