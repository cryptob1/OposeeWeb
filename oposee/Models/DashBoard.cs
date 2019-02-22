using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace oposee.Models
{
    public class DashBoard
    {
        public int UserCount { get; set; }
        public int QuestionCount { get; set; }
        public int AnsweredQuestion { get; set; }
        public int UnAnsweredQues { get; set; }
    }
    [DataContract]
    public class Point
    {
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public Nullable<double> X = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }
}