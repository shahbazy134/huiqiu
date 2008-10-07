using System;
using System.Collections.Generic;
using System.Text;

namespace PartialClassInterfaces
{
    public partial class TriValue
    {
        public decimal First { get; set; }
        public decimal Second { get; set; }
        public decimal Third { get; set; }

        public TriValue(decimal first, decimal second, decimal third)
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public decimal Average
        {
            get { return (Sum / 3); }
        }

        public decimal Sum
        {
            get { return First + Second + Third; }
        }

        public decimal Product
        {
            get { return First * Second * Third; }
        }
    }
}
