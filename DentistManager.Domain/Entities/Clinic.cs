//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DentistManager.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Clinic
    {
        public Clinic()
        {
            this.Appointments = new HashSet<Appointment>();
            this.CustomMaterials = new HashSet<CustomMaterial>();
            this.Doctors = new HashSet<Doctor>();
            this.Patients = new HashSet<Patient>();
            this.PaymentReceipts = new HashSet<PaymentReceipt>();
            this.Secretaries = new HashSet<Secretary>();
            this.Storages = new HashSet<Storage>();
            this.Treatments = new HashSet<Treatment>();
        }
    
        public int ClinicID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public Nullable<bool> Acitve { get; set; }
    
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<CustomMaterial> CustomMaterials { get; set; }
        public virtual ICollection<Doctor> Doctors { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
        public virtual ICollection<PaymentReceipt> PaymentReceipts { get; set; }
        public virtual ICollection<Secretary> Secretaries { get; set; }
        public virtual ICollection<Storage> Storages { get; set; }
        public virtual ICollection<Treatment> Treatments { get; set; }
    }
}
