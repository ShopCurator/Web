using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    [Serializable]
    public class Product
    {
        public string Name
        {
            get;
            set;
        }
        public string Brand
        {
            get;
            set;
        }
        public string SellerCompany
        {
            get;
            set;
        }
        public string Price
        {
            get;
            set;
        }
    }
}
