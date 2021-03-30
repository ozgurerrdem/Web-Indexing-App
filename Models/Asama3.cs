using System;
using System.Collections.Generic;
namespace yazlab.webui.Models
{
    public class Asama3
    {
        public List<Tuple<string,int>> text1 = new List<Tuple<string,int>>();
        public List<Tuple<string,int>> text2 = new List<Tuple<string,int>>();
        public int benzerlikSkoru { get; set; }
    }
}