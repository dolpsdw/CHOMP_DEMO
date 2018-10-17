namespace CHOMP_DEMO.DTO
{
    public class dbsp_GetProspectCustomerInfoByCustomerID_Result
    {
        public string ProspectID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int CustomerProspectID { get; set; }
        public string ProspectPassword { get; set; }
        public double ProspectFreePlayAmount { get; set; }
        public bool FreePlayValidated { get; set; }
    }
}