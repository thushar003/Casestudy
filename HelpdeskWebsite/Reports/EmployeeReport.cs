using HelpdeskViewModels;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace HelpdeskWebsite.Reports
{
    public class EmployeeReport
    {
        public async void GenerateReport(string rootpath)
        {
            PageSize pg = PageSize.A4;
            var helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);
            PdfWriter writer = new(rootpath + "/pdfs/empRep.pdf",
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new(writer);
            Document document = new(pdf); // PageSize(595, 842)
            document.Add(new Image(ImageDataFactory.Create(rootpath + "/img/helpdesk.jpg"))
                .ScaleAbsolute(200, 100)
                .SetFixedPosition(((pg.GetWidth() - 200) / 2), 710));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Current Employees")
                .SetFont(helvetica)
                .SetFontSize(24)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph(""));
            Table table = new(3);
            table
                .SetWidth(298) // roughly 50%
                .SetTextAlignment(TextAlignment.CENTER)
                .SetRelativePosition(0, 0, 0, 0)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);
            table.AddCell(new Cell().Add(new Paragraph("Title")
                    .SetFontSize(16)
                    .SetBold()
                    .SetPaddingLeft(18)
                    .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("First Name")
                    .SetFontSize(16)
                    .SetBold()
                    .SetPaddingLeft(16)
                    .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Last Name")
                    .SetBold()
                    .SetFontSize(16)
                    .SetPaddingLeft(16)
                    .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));


            EmployeeViewModel svm = new();
            List<EmployeeViewModel> employees1 = await svm.GetAll();

            foreach (var employee in employees1)
            {
                table.AddCell(new Cell().Add(new Paragraph(employee.Title)
                        .SetFontSize(14)
                        .SetPaddingLeft(24)
                        .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(employee.Firstname)
                        .SetFontSize(14)
                        .SetPaddingLeft(24)
                        .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(employee.Lastname)
                        .SetFontSize(14)
                        .SetPaddingLeft(24)
                        .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER));
            }
            document.Add(table);
            document.Add(new Paragraph("Employee report written on - " + DateTime.Now)
                .SetFontSize(6)
                .SetTextAlignment(TextAlignment.CENTER));
            document.Close();
        }
    }
}
