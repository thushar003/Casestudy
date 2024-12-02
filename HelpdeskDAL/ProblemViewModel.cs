using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class ProblemViewModel
    {
        private readonly ProblemDAO _dao;
        public string? Description { get; set; }

        // constructor
        public ProblemViewModel()
        {
            _dao = new ProblemDAO();
        }

        public async Task GetByDescription()
        {
            try
            {
                Problem prob = await _dao.GetByDescription((string)Description!);

            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Description = "unknown";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<ProblemViewModel>> GetAll()
        {
            List<ProblemViewModel> allVms = new();
            try
            {
                List<Problem> allProblems = await _dao.GetAll();
                foreach (Problem emp in allProblems)
                {
                    ProblemViewModel empVm = new()
                    {
                        Description = emp.Description,
                    };
                    allVms.Add(empVm);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return allVms;
        }
    }
}
