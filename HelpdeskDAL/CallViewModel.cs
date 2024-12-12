/*
 * File: CallViewModel.cs
 * @author: Thushar Joseph Joji, 1190586
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class CallViewModel
    {
        private readonly CallDAO _dao;
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ProblemId { get; set; }
        public string? EmployeeName { get; set; }
        public string? ProblemDescription { get; set; }
        public string? TechName { get; set; }
        public int TechId { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? DateClosed { get; set; }
        public bool OpenStatus { get; set; }
        public string? Notes { get; set; }
        public string? Timer { get; set; }

        // constructor
        public CallViewModel()
        {
            _dao = new CallDAO();
        }

        public async Task GetById()
        {
            try
            {
                Call call = await _dao.GetById((int)Id!);
                Id = call.Id;
                EmployeeId = call.EmployeeId;
                EmployeeName = call.Employee.LastName;
                ProblemId = call.ProblemId;
                ProblemDescription = call.Problem.Description;
                TechId = call.TechId;
                TechName = call.Tech.LastName;
                DateOpened = call.DateOpened;
                DateClosed = call.DateClosed;
                OpenStatus = call.OpenStatus;
                Timer = Convert.ToBase64String(call.Timer!);
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Id = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<CallViewModel>> GetAll()
        {
            List<CallViewModel> allVms = new();
            try
            {
                List<Call> allCalls = await _dao.GetAll();
                foreach (Call call in allCalls)
                {
                    CallViewModel empVm = new()
                    {
                        Id = call.Id,
                        EmployeeId = call.EmployeeId,
                        EmployeeName = call.Employee.LastName,
                        ProblemId = call.ProblemId,
                        ProblemDescription = call.Problem.Description,
                        TechId = call.TechId,
                        TechName = call.Tech.LastName,
                        DateOpened = call.DateOpened,
                        DateClosed = call.DateClosed,
                        OpenStatus = call.OpenStatus,
                        Timer = Convert.ToBase64String(call.Timer!)
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

        public async Task Add()
        {
            Id = -1;
            try
            {
                Call call = new()
                {
                    EmployeeId = EmployeeId,
                    ProblemId = ProblemId,
                    TechId = TechId,
                    DateOpened = DateOpened,
                    DateClosed = DateClosed,
                    OpenStatus = OpenStatus,
                    Notes = Notes!,
                };
                Id = await _dao.Add(call);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<int> Update()
        {
            int updateStatus;
            try
            {
                if (!this.OpenStatus)
                {
                    this.DateClosed = DateTime.Now;
                }
                Call call = new()
                {
                    Id = (int)Id!,
                    EmployeeId = EmployeeId,
                    ProblemId = ProblemId,
                    TechId = TechId,
                    DateOpened = DateOpened,
                    DateClosed = DateClosed,
                    OpenStatus = OpenStatus,
                    Notes = Notes!,
                    Timer = Convert.FromBase64String(Timer!)
                };
                updateStatus = -1; // start out with a failed state
                updateStatus = Convert.ToInt16(await _dao.Update(call)); // overwrite status
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return updateStatus;
        }

        public async Task<int> Delete()
        {
            try
            {
                // dao will return # of rows deleted
                return await _dao.Delete(Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }
    }
}
