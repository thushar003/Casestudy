using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class ProblemDAO
    {
        readonly IRepository<Problem> _repo;

        public ProblemDAO()
        {
            _repo = new HelpdeskRepository<Problem>();
        }

        public async Task<Problem> GetByDescription(string description)
        {
            Problem? selectedProblem;
            try
            {
                //HelpdeskContext _db = new();
                selectedProblem = await _repo.GetOne(emp => emp.Description == description);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " +
                                ex.Message);
                throw;
            }

            return selectedProblem!;
        }

        public async Task<List<Problem>> GetAll()
        {
            List<Problem> allProblems;
            try
            {
                //HelpdeskContext _db = new();
                allProblems = await _repo.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return allProblems;
        }
    }
}
