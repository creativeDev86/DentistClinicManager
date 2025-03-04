﻿using DentistManager.Domain.DAL.Abstract;
using DentistManager.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DentistManager.Domain.BL.Abstract;
using Microsoft.AspNet.Identity;
using DentistManager.DentistUI.Infrastructure;

namespace DentistManager.DentistUI.Areas.DoctorDashboard.Controllers
{
    //[Authorize(Roles = "Doctor")]
    public class TreatmentSessionController : Controller
    {
        IOpperationRepository opperationRepository;
        ITreatmentRepository treatmentRepository;
        ITreatmentBL treatmentBL;
        IMaterialRepository materialRepository;
        ISessionStateManger sessionStateManger;
        IDoctorRepository doctorRepository;
        IAppointmentRepository appointmentRepository;
        IMatrailBL matrailBL;
        public TreatmentSessionController(IOpperationRepository _opperationRepository, ITreatmentRepository _treatmentRepository, ITreatmentBL _treatmentBL, IMaterialRepository _materialRepository, ISessionStateManger _sessionStateManger, IDoctorRepository _doctorRepository, IAppointmentRepository _appointmentRepository, IMatrailBL _matrailBL)
        {
            opperationRepository = _opperationRepository;
            treatmentRepository = _treatmentRepository;
            treatmentBL = _treatmentBL;
            materialRepository = _materialRepository;
            sessionStateManger = _sessionStateManger;
            doctorRepository = _doctorRepository;
            appointmentRepository = _appointmentRepository;
            matrailBL = _matrailBL;
        }

        //
        // GET: /DoctorDashboard/TreatmentSession/

        [NonAction]
        public int getCurrentPatientID()
        {
            var aa = User.Identity.GetUserId();
            return int.Parse(sessionStateManger.getDoctorActivePatinet(User.Identity.GetUserId()));
        }

        [NonAction]
        public int getUserCurrentClinecID()
        {
            //var aa = sessionStateManger.getClinecIDForCurrentDoctor(User.Identity.GetUserId());
            return sessionStateManger.getClinecIDForCurrentDoctor(User.Identity.GetUserId());
        }


        public ActionResult Index()
        {
            return View();
        }
        //  /DoctorDashboard/TreatmentSession/TreatmentSessionMain
        public ActionResult TreatmentSessionMain(int AppointmentID=0)
        {
            // get last appointment date by patient and doctor id
            int patientID = getCurrentPatientID();

            if(AppointmentID == 0)
               AppointmentID = appointmentRepository.getLastAppointmentIDByPatientID(patientID);
            
            string appointmentDate = appointmentRepository.getAppointmentDateByID(AppointmentID);
            if (appointmentDate == string.Empty)
                return Redirect("~/Appoiment/Appoiments.aspx");

            ViewBag.AppointmentID = AppointmentID;
            ViewBag.DoctorID = doctorRepository.getClinecIDByUserID(User.Identity.GetUserId());
            ViewBag.PatientID = patientID;
            ViewBag.appointmentDate = appointmentDate; 
            
            return View();
        }

        public ActionResult opperationList()
        {
            var opperationList = opperationRepository.getOpperationList();
            return PartialView(opperationList);
        }



        //   /DoctorDashboard/TreatmentSession/GetTreatmentListByPatientID
        [HttpPost]
        public ActionResult GetTreatmentListByPatientID(int patientID = 0)
        {
            IEnumerable<TreatmentPresntViewModel> treatmentViewModel = treatmentRepository.getPatientPresntTreatmentList(patientID);
            return Json(treatmentViewModel);
        }

        [HttpPost]
        public ActionResult GetMatrialList(int treatmentID = 0)
        {
           IEnumerable<MaterialMiniViewModel> matrailList=  materialRepository.getMatrailMiniList();
           return Json(matrailList);
        }

        [HttpPost]
        public ActionResult GetTreatmentMatrialList(int treatmentID = 0)
        {
            IEnumerable<MaterialMiniPresentViewModel> matrailList = materialRepository.getTreatmentMatrailList(treatmentID);
            return Json(matrailList);
        }

        [HttpPost]
        public void RemoveTreatmentItem(int treatmentID = 0)
        {
            treatmentRepository.RemoveTreatmentByID(treatmentID);
        }

        [HttpPost]
        public void TreatmentSave(IEnumerable<TreatmentPresntViewModel> treatmentList, int appointmentID=0)
        {
            int patientID = getCurrentPatientID();
            int AppointmentID = appointmentID;
            int clinecID = getUserCurrentClinecID();
            int DoctorID = doctorRepository.getDoctorIDByUserID(User.Identity.GetUserId());

            treatmentBL.saveTreatment(treatmentList,AppointmentID,DoctorID,patientID,clinecID);

            //return RedirectToAction("TreatmentSessionMain");
        }

        [HttpPost]
        public bool matrailTreatmentSave(MatrailToSaveWrapViewModel matrailToSave)
        {
            if (matrailToSave.treatmentID == 0)
                return false;
            if (matrailToSave.vmMatrailListToSave == null)
                return false;
            if (matrailToSave.vmMatrailListToSave.Count() == 0)
                return false;

            int clinecID=getUserCurrentClinecID();
           
            bool check = treatmentBL.SaveMatrailOfTreatment(matrailToSave.vmMatrailListToSave, matrailToSave.treatmentID, clinecID);

            return check;
        }

        [HttpPost]
        public bool removeTreatmentMatrail(int matrailID = 0, int treatmentID = 0)
        {
            if (treatmentID == 0)
                return false;
            if (matrailID == 0)
                return false;
            bool check;
            int clinecID = getUserCurrentClinecID();
            check=matrailBL.RemoveMatrailofTreatment(clinecID, matrailID, treatmentID);
           
            return check;
        }
	}
}