using System.Collections.Generic;
using System.Threading.Tasks;
using GithubModels;

namespace RepoSkillMiner.Services
{
    public interface IExcelExportService
    {
        Task ExportToExcel(List<AuthorsAndTechs> authorsAndTechs);
    }
}
