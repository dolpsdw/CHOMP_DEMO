using System;

namespace CHOMP_DEMO.DTO
{
    public class dbsp_GetCustomerInfoAtLogin_Result
    {
        public string CustomerID { get; set; }
        public string NameLast { get; set; }
        public string NameFirst { get; set; }
        public string Password { get; set; }
        public string ParlayName { get; set; }
        public string Store { get; set; }
        public Nullable<int> PercentBook { get; set; }
        public string CreditAcctFlag { get; set; }
        public string AgentID { get; set; }
        public Nullable<double> LossCap { get; set; }
        public string Currency { get; set; }
        public Nullable<double> ParlayMaxBet { get; set; }
        public Nullable<double> ParlayMaxPayout { get; set; }
        public string Active { get; set; }
        public Nullable<int> ConfirmationDelay { get; set; }
        public Nullable<int> TimeZone { get; set; }
        public string EMail { get; set; }
        public string CommentsForCustomer { get; set; }
        public string CasinoActiveFlag { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string Fax { get; set; }
        public Nullable<System.DateTime> OpenDateTime { get; set; }
    }
}