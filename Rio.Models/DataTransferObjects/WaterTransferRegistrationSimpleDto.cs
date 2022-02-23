using System;

namespace Rio.Models.DataTransferObjects
{
    public partial class WaterTransferRegistrationSimpleDto
    {
        public AccountSimpleDto Account { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsPending { get; set; }
    }
}