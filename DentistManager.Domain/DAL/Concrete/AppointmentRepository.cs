﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DentistManager.Domain.DAL.Abstract;
using DentistManager.Domain.ViewModel;
using DentistManager.Domain.Entities;

namespace DentistManager.Domain.DAL.Concrete
{
    public class AppointmentRepository : IAppointmentRepository
    {
        // for api
        public IEnumerable<patientAppoInfo> getPatientAppountmenInfo(int patientID, string mobile)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {

                Patient patient = ctx.Patients.Where(x => x.PatientID == patientID && x.Mobile == mobile).FirstOrDefault();

                if (patient == null)
                    return null;

                IEnumerable<patientAppoInfo> appointmentViewModel;

                var appointmentsIQ = ctx.Appointments;
                var doctorIQ = ctx.Doctors;

                appointmentViewModel = (from a in appointmentsIQ
                                        join d in doctorIQ on a.DoctorID equals d.DoctorID
                                        where a.PatientID == patientID
                                        select new patientAppoInfo {  AppDate=a.Start_date, DoctorName=d.Name}).ToList();

                return appointmentViewModel;
            }
        }

        // this is a mini list i use it in drop down list
        public IEnumerable<AppointmentViewModel> getPatientAppountmentList(int patientID)
        {
            using(Entities.Entities ctx=new Entities.Entities ())
            {
                IEnumerable<AppointmentViewModel> appointmentViewModel;

                var appointmentsIQ = ctx.Appointments;

                appointmentViewModel = (from a in appointmentsIQ
                                        where a.PatientID == patientID
                                        select new AppointmentViewModel {  AppointmentID=a.AppointmentID , Date=a.Start_date }).ToList();
                return appointmentViewModel;
            }
        }

        public List<AppointmentViewModelFull> getClinecAppointmentList(int ClinecID)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {
                List<AppointmentViewModelFull> appointmentViewModelFull;

                var appointmentsIQ = ctx.Appointments;

                appointmentViewModelFull =  (from a in appointmentsIQ
                                             where a.ClinicID == ClinecID
                                             select new AppointmentViewModelFull { id= a.AppointmentID ,ClinicID= a.ClinicID, DoctorID=a.DoctorID, PatientID=a.PatientID, Reason=a.Reason , Status= a.Status, start_date=a.Start_date, end_date=a.End_date, text=a.Text  }).ToList();
                return appointmentViewModelFull;
            }
        }


        public int AddNewAppointment(AppointmentViewModelFull appointment)
        {
            int appointmentID = 0;
            using (Entities.Entities ctx = new Entities.Entities())
            {
                
                Appointment appointmentEntity = ctx.Appointments.Create();
                appointmentEntity.ClinicID = appointment.ClinicID;
                appointmentEntity.DoctorID = appointment.DoctorID;
                appointmentEntity.PatientID = appointment.PatientID;
                appointmentEntity.Reason = appointment.Reason;
                appointmentEntity.Status = appointment.Status;
                appointmentEntity.Start_date = appointment.start_date;
                appointmentEntity.End_date = appointment.end_date;
                appointmentEntity.Text = appointment.text;

                ctx.Appointments.Add(appointmentEntity);
                ctx.SaveChanges();
                appointmentID = appointmentEntity.AppointmentID;
            }
            return appointmentID;
        }


        public bool alterAppointment(AppointmentViewModelFull appointment)
        {
            int count = 0;
            using (Entities.Entities ctx = new Entities.Entities())
            {
                Appointment appointmentEntity = ctx.Appointments.Find(appointment.id);

                if (appointmentEntity == null)
                    return false;

                appointmentEntity.ClinicID = appointment.ClinicID;
                appointmentEntity.DoctorID = appointment.DoctorID;
                appointmentEntity.PatientID = appointment.PatientID;
                appointmentEntity.Reason = appointment.Reason;
                appointmentEntity.Status = appointment.Status;
                appointmentEntity.Start_date = appointment.start_date;
                appointmentEntity.End_date = appointment.end_date;
                appointmentEntity.Text = appointment.text;

                ctx.Entry(appointmentEntity).State = System.Data.Entity.EntityState.Modified;
                count = ctx.SaveChanges();
            }
            return count > 0 ? true : false;
        }

        public bool deleteAppointment(int appointmentID)
        {
            int count = 0;
            using (Entities.Entities ctx = new Entities.Entities())
            {
                Appointment appointment = ctx.Appointments.Find(appointmentID);
                if (appointment != null)
                {
                    ctx.Appointments.Remove(appointment);
                    count = ctx.SaveChanges();
                }
            }
            return count > 0 ? true : false;
        }


        public string getAppointmentDateByID(int appointmentID)
        {
            string appointmentDate=string.Empty;
            using (Entities.Entities ctx = new Entities.Entities())
            {
                appointmentDate = ctx.Appointments.Where(x => x.AppointmentID == appointmentID).Select(x => x.Start_date).FirstOrDefault().ToString();

                if (appointmentDate == "1/1/0001 12:00:00 AM")
                    appointmentDate = string.Empty;
            }
            return appointmentDate;
        }


        public int getLastAppointmentIDByPatientID(int patientID)
        {
            int appointmentID = 0;
           
            using (Entities.Entities ctx = new Entities.Entities())
            {
                var appointmentIQ = ctx.Appointments;

                appointmentID = (from a in appointmentIQ
                                 orderby a.Start_date descending
                                 where a.PatientID == patientID
                                 select a.AppointmentID).FirstOrDefault();

                //appointmentID = ctx.Appointments.Where(x => x.PatientID == patientID).Select(x => x.AppointmentID).LastOrDefault();
            }
            return appointmentID;
        }


        public List<AppointmentStatusViewModel> getDoctorDailyMeeting(int ClinecID, int DoctorID)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {
                List<AppointmentStatusViewModel> appointmentViewModelFull;

                var appointmentsIQ = ctx.Appointments;
                var patientIQ = ctx.Patients;

                appointmentViewModelFull = (from a in appointmentsIQ
                                            join p in patientIQ on a.PatientID equals p.PatientID
                                            where a.ClinicID == ClinecID && a.DoctorID == DoctorID && a.Start_date.Day == DateTime.Now.Day
                                            select new AppointmentStatusViewModel { id = a.AppointmentID, PatientID = a.PatientID, Status = a.Status, start_date = a.Start_date , PatientName=p.Name }).ToList();
                return appointmentViewModelFull;
            }
        }
        public List<AppointmentStatusViewModel> getDoctorDailyMeeting(int ClinecID, int DoctorID, string status)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {
                List<AppointmentStatusViewModel> appointmentViewModelFull;

                var appointmentsIQ = ctx.Appointments;
                var patientIQ = ctx.Patients;

                appointmentViewModelFull = (from a in appointmentsIQ
                                            join p in patientIQ on a.PatientID equals p.PatientID
                                            where a.ClinicID == ClinecID && a.DoctorID == DoctorID && a.Start_date.Day == DateTime.Now.Day && a.Status == status
                                            select new AppointmentStatusViewModel { id = a.AppointmentID, PatientID = a.PatientID, Status = a.Status, start_date = a.Start_date, PatientName = p.Name }).ToList();
               
                return appointmentViewModelFull;
            }
        }

        public List<AppointmentStatusViewModel> getDoctorWeeklyMeeting(int ClinecID, int DoctorID)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {
                List<AppointmentStatusViewModel> appointmentViewModelFull;
                DateTime todate=DateTime.Now.AddDays(7);
                var appointmentsIQ = ctx.Appointments;
                var patientIQ = ctx.Patients;

                appointmentViewModelFull = (from a in appointmentsIQ
                                            join p in patientIQ on a.PatientID equals p.PatientID
                                            where a.ClinicID == ClinecID && a.DoctorID == DoctorID && a.Start_date >= DateTime.Now && a.Start_date <= todate
                                            select new AppointmentStatusViewModel { id = a.AppointmentID, PatientID = a.PatientID, Status = a.Status, start_date = a.Start_date, PatientName = p.Name }).ToList();
                return appointmentViewModelFull;
            }
        }

        public List<AppointmentStatusViewModel> getDoctorWeeklyMeeting(int ClinecID, int DoctorID, string status)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {
                List<AppointmentStatusViewModel> appointmentViewModelFull;
                DateTime todate = DateTime.Now.AddDays(7);
                var appointmentsIQ = ctx.Appointments;
                var patientIQ = ctx.Patients;

                appointmentViewModelFull = (from a in appointmentsIQ
                                            join p in patientIQ on a.PatientID equals p.PatientID
                                            where a.ClinicID == ClinecID && a.DoctorID == DoctorID && a.Start_date >= DateTime.Now && a.Start_date <= todate && a.Status == status
                                            select new AppointmentStatusViewModel { id = a.AppointmentID, PatientID = a.PatientID, Status = a.Status, start_date = a.Start_date, PatientName = p.Name }).ToList();
                return appointmentViewModelFull;
            }
        }

        public List<AppointmentStatusViewModel> getDoctorMonthlyMeeting(int ClinecID, int DoctorID)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {
                List<AppointmentStatusViewModel> appointmentViewModelFull;
                DateTime todate = DateTime.Now.AddMonths(1);
                var appointmentsIQ = ctx.Appointments;
                var patientIQ = ctx.Patients;

                appointmentViewModelFull = (from a in appointmentsIQ
                                            join p in patientIQ on a.PatientID equals p.PatientID
                                            where a.ClinicID == ClinecID && a.DoctorID == DoctorID && a.Start_date >= DateTime.Now && a.Start_date <= todate
                                            select new AppointmentStatusViewModel { id = a.AppointmentID, PatientID = a.PatientID, Status = a.Status, start_date = a.Start_date, PatientName = p.Name }).ToList();
                return appointmentViewModelFull;
            }
        }

        public List<AppointmentStatusViewModel> getDoctorMonthlyMeeting(int ClinecID, int DoctorID, string status)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {
                List<AppointmentStatusViewModel> appointmentViewModelFull;
                DateTime todate = DateTime.Now.AddMonths(1);

                var appointmentsIQ = ctx.Appointments;
                var patientIQ = ctx.Patients;

                appointmentViewModelFull = (from a in appointmentsIQ
                                            join p in patientIQ on a.PatientID equals p.PatientID
                                            where a.ClinicID == ClinecID && a.DoctorID == DoctorID && a.Start_date >= DateTime.Now && a.Start_date <= todate && a.Status == status
                                            select new AppointmentStatusViewModel { id = a.AppointmentID, PatientID = a.PatientID, Status = a.Status, start_date = a.Start_date, PatientName = p.Name }).ToList();
                return appointmentViewModelFull;
            }
        }


        public List<AppointmentStatusViewModel> getClinecMeeting(int ClinecID, DateTime begin, DateTime ending)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {
                List<AppointmentStatusViewModel> appointmentViewModelFull;

                var appointmentsIQ = ctx.Appointments;
                var patientIQ = ctx.Patients;
                var doctorIQ = ctx.Doctors;

                appointmentViewModelFull = (from a in appointmentsIQ
                                            join p in patientIQ on a.PatientID equals p.PatientID
                                            join d in doctorIQ on a.DoctorID equals d.DoctorID
                                            where a.ClinicID == ClinecID && a.Start_date >= begin && a.Start_date <= ending
                                            select new AppointmentStatusViewModel { id = a.AppointmentID, PatientID = a.PatientID, Status = a.Status, start_date = a.Start_date, PatientName = p.Name, DoctorName=d.Name }).ToList();
                return appointmentViewModelFull;
            }
        }

        public List<AppointmentStatusViewModel> getClinecMeeting(int ClinecID, string status, DateTime begin, DateTime ending)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {
                List<AppointmentStatusViewModel> appointmentViewModelFull;

                var appointmentsIQ = ctx.Appointments;
                var patientIQ = ctx.Patients;
                var doctorIQ = ctx.Doctors;

                appointmentViewModelFull = (from a in appointmentsIQ
                                            join p in patientIQ on a.PatientID equals p.PatientID
                                            join d in doctorIQ on a.DoctorID equals d.DoctorID
                                            where a.ClinicID == ClinecID && a.Start_date >= begin && a.Start_date <= ending && a.Status == status
                                            select new AppointmentStatusViewModel { id = a.AppointmentID, PatientID = a.PatientID, Status = a.Status, start_date = a.Start_date, PatientName = p.Name , DoctorName= d.Name}).ToList();
                return appointmentViewModelFull;
            }
        }


        public AppointmentStatusViewModel getPatientStatus(int appointmentID)
        {
            using (Entities.Entities ctx = new Entities.Entities())
            {
                AppointmentStatusViewModel appointmentViewModelFull;

                var appointmentsIQ = ctx.Appointments;
                var patientIQ = ctx.Patients;
                var doctorIQ = ctx.Doctors;

                appointmentViewModelFull = (from a in appointmentsIQ
                                            join p in patientIQ on a.PatientID equals p.PatientID
                                            join d in doctorIQ on a.DoctorID equals d.DoctorID
                                            where a.AppointmentID == appointmentID
                                            select new AppointmentStatusViewModel { id = a.AppointmentID, PatientID = a.PatientID, Status = a.Status, start_date = a.Start_date, PatientName = p.Name, DoctorName = d.Name }).FirstOrDefault();
                return appointmentViewModelFull;
            }
        }


        public bool updatePatientStatus(AppointmentStatusViewModel appointmentStatusViewModel)
        {
            int count = 0;
            using (Entities.Entities ctx = new Entities.Entities())
            {
                Appointment appointmentEntity = ctx.Appointments.Find(appointmentStatusViewModel.id);

                if (appointmentEntity == null)
                    return false;

                appointmentEntity.Status = appointmentStatusViewModel.Status;

                ctx.Entry(appointmentEntity).State = System.Data.Entity.EntityState.Modified;
                count = ctx.SaveChanges();
            }
            return count > 0 ? true : false;
        }
    }
}
