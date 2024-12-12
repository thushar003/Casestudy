/*
 * File: ProblemViewModel.cs
 * @author: Thushar Joseph Joji, 1190586
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace HelpdeskDAL
{
    public class ProblemViewModel
    {
        private readonly ProblemDAO _dao;

        public int Id { get; set; }
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

        public async Task<int> Update()
        {
            int updateStatus;
            try
            {
                Problem prob = new()
                {
                    Description = Description
                };
                updateStatus = -1; // start out with a failed state
                updateStatus = Convert.ToInt16(await _dao.Update(prob)); // overwrite status
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return updateStatus;
        }
    }
}
