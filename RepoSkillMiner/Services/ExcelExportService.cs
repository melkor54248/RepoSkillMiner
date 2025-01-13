using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GithubModels;
using OfficeOpenXml;

namespace RepoSkillMiner.Services
{
    public class ExcelExportService : IExcelExportService
    {
        public async Task ExportToExcel(List<AuthorsAndTechs> authorsAndTechs)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("AuthorsAndTechs");

                // Add headers
                worksheet.Cells[1, 1].Value = "Login";
                worksheet.Cells[1, 2].Value = "Avatar URL";
                worksheet.Cells[1, 3].Value = "Technologies";

                // Add data
                for (int i = 0; i < authorsAndTechs.Count; i++)
                {
                    var author = authorsAndTechs[i];
                    worksheet.Cells[i + 2, 1].Value = author.Login;
                    worksheet.Cells[i + 2, 2].Value = author.Avatar_url;
                    worksheet.Cells[i + 2, 3].Value = string.Join(", ", author.Technologies.Select(t => $"{t.Name} ({t.Count})"));
                }

                // Save the file
                var file = new FileInfo("AuthorsAndTechs.xlsx");
                await package.SaveAsAsync(file);
            }
        }
    }
}
