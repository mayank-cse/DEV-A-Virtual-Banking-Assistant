using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevVirtualBankingAssistant.Models;
using Microsoft.Bot.Schema;
using Entity = Microsoft.Bot.Schema.Entity;

namespace DevVirtualBankingAssistant.Helper
{
    public class AIFormRecognizer
    {
        IConfiguration configuration;

        public AIFormRecognizer(IConfiguration config)
        {
            configuration = config;
        }


        //Uses the 
        public async Task<InvoiceData> ProcessAttachmentAsync(Uri attachmentUri)
        {

            try
            {


                string endpoint = configuration["FormRecognizerEndpoint"];
                string apiKey = configuration["FormRecognizerAPIKey"];
                var credential = new AzureKeyCredential(apiKey);
                var client = new FormRecognizerClient(new Uri(endpoint), credential);


                var options = new RecognizeInvoicesOptions() { Locale = "en-US", IncludeFieldElements = true };

                RecognizeInvoicesOperation operation = await client.StartRecognizeInvoicesFromUriAsync(attachmentUri, options);
                Response<RecognizedFormCollection> operationResponse = await operation.WaitForCompletionAsync();
                RecognizedFormCollection invoices = operationResponse.Value;

                // To see the list of the supported fields returned by service and its corresponding types, consult:
                // https://aka.ms/formrecognizer/invoicefields

                RecognizedForm invoice = invoices.Single();


                //   Entity invoiceEnt = GetInvoiceEntityFromForm(invoice);

                string customerName = null;
                if (invoice.Fields.TryGetValue("CustomerName", out FormField customerNameField))
                {
                    if (customerNameField.Value.ValueType == FieldValueType.String)
                    {
                        customerName = customerNameField.Value.AsString();

                    }
                }

                List<Entity> invoiceLines = GetInvoiceLinesFromForm(invoice);
                InvoiceData invoiceData = new InvoiceData
                {
                    CustomerName = customerName,
                    //InvoiceLines = invoiceLines
                };
                return invoiceData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private Entity GetInvoiceEntityFromForm(RecognizedForm invoice)
        {
            Entity invoiceEnt = new Entity("invoice");



            if (invoice.Fields.TryGetValue("CustomerName", out FormField customerNameField))
            {
                if (customerNameField.Value.ValueType == FieldValueType.String)
                {
                    string customerName = customerNameField.Value.AsString();
                    Console.WriteLine($"Customer Name: '{customerName}', with confidence {customerNameField.Confidence}");
                }
            }

            if (invoice.Fields.TryGetValue("InvoiceTotal", out FormField invoiceTotalField))
            {
                if (invoiceTotalField.Value.ValueType == FieldValueType.Float)
                {
                    float invoiceTotal = invoiceTotalField.Value.AsFloat();
                    Console.WriteLine($"Invoice Total: '{invoiceTotal}', with confidence {invoiceTotalField.Confidence}");
                }
            }

            return invoiceEnt;
        }


        private List<Entity> GetInvoiceLinesFromForm(RecognizedForm invoice)
        {
            List<Entity> invoiceLines = new List<Entity>();


            FormPage invoicePage = invoice.Pages.FirstOrDefault();

            if (invoicePage == null || invoicePage.Tables == null)
                return null;

            List<FormTable> formTables = invoicePage.Tables.ToList();

            FormTable invoiceLineTable = formTables.Where(x => x.Cells != null && x.Cells.Any(y => y.Text != null && y.Text.ToLower().Contains("quantity"))).FirstOrDefault();

            if (invoiceLineTable == null)
                return null;

            List<FormTableCell> invoiceLineCells = invoiceLineTable.Cells.ToList();

            for (var rowIndex = 1; rowIndex < invoiceLineTable.RowCount; rowIndex++)
            {
                FormTableCell quantityCell = invoiceLineCells.FirstOrDefault(x => x.RowIndex == rowIndex && x.ColumnIndex == 0);
                decimal quantity = decimal.Parse(quantityCell.Text);

                FormTableCell descCell = invoiceLineCells.FirstOrDefault(x => x.RowIndex == rowIndex && x.ColumnIndex == 1);
                string prodDescription = descCell.Text;

                FormTableCell unitPriceCell = invoiceLineCells.FirstOrDefault(x => x.RowIndex == rowIndex && x.ColumnIndex == 2);
                decimal unitprice = decimal.Parse(unitPriceCell.Text.Replace("$", ""));


                /*Entity invoiceLine = new Entity("invoicedetail");
                invoiceLine["quantity"] = quantity;
                invoiceLine["productdescription"] = prodDescription;
                invoiceLine["priceperunit"] = new Money(unitprice);
                invoiceLines.Add(invoiceLine);*/
            }

            return invoiceLines;

        }
    }
}
