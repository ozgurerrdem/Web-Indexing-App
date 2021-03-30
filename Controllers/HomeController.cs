using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using yazlab.webui.Models;

namespace yazlab.webui.Controllers
{
    
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
         
        public IActionResult Asama1(String url)
        {         
            Asama1 p = new Asama1();
            List<string> temp = kelimeleriBul(url);
            foreach (var grp in temp.GroupBy(i => i))
            {  
                p.text.Add(new Tuple<string,int>(grp.Key, grp.Count()));
            }
            return View(p);
        }
        public IActionResult Asama2(String url)
        {   
            Asama2 p = new Asama2();
            List<string> temp = kelimeleriBul(url);
            foreach (var grp in temp.GroupBy(i => i))
            {  
                p.text.Add(new Tuple<string,int>(grp.Key, grp.Count()));
            }
            p.text.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            return View(p);
        }       

        public IActionResult Asama3(String url1, String url2)
        {   
            ViewBag.url1 = url1;
            ViewBag.url2 = url2;
            Asama3 p = new Asama3();
            List<string> temp1 = kelimeleriBul(url1);
            foreach (var grp in temp1.GroupBy(i => i))
            {  
                p.text1.Add(new Tuple<string,int>(grp.Key, grp.Count()));
            }
            p.text1.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            List<string> temp2 = kelimeleriBul(url2);
            foreach (var grp in temp2.GroupBy(i => i))
            {  
                p.text2.Add(new Tuple<string,int>(grp.Key, grp.Count()));
            }
            p.text2.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            int benzeme=0;
            for(int i = 0; i < p.text1.Count; i++)
            {
                for(int j = 0; j < p.text1.Count; j++)
                {
                    if(p.text1[i].Item1.Equals(p.text2[j].Item1))
                    {
                        benzeme++;
                    }
                    if(j==4)break;
                }
                if(i==4)break;
            }
            p.benzerlikSkoru = Convert.ToInt32((benzeme*100)/5);
            return View(p);
        }
        public IActionResult Asama4(String url)
        {   
            ViewBag.kontrol = true;
            Asama4 p = new Asama4();
            string text = linkBul(url);
            string[] split = text.Split(' ');
            if(p.Links1 == null)
            {
                p.Links1 = new List<string>();
            }
            if(p.Links2 == null)
            {
                p.Links2 = new List<string>();
            }
            foreach(var item in split)
            {
                if(p.Links1.Count == 3) break;
                else
                {
                    if(item.Length>8 && item != null)
                    {
                        if(item.Substring(0,8).Equals("https://"))
                        {
                            p.Links1.Add(item);
                        }
                    }
                }
            }           
            for(int i =0; i<p.Links1.Count; i++)
            {
                text = linkBul(p.Links1[i]);
                split = text.Split(' ');
                p.Links2.Add(p.Links1[i]);
                foreach(var item in split)
                {                    
                    if(item.Length>8)
                    {
                        if(item.Substring(0,8).Equals("https://"))
                        {
                            Console.WriteLine("dewam"+i.ToString()+"/"+p.Links1.Count.ToString());
                            p.Links2.Add(item);
                            if(p.Links2.Count % 3 == 0) break;  
                        }
                    }                                      
                }
            }
            return View(p);
        }

        private string linkBul(String url)
        {
            string text="";
            try
            {
                var client = new WebClient();
                var htmlSource = client.DownloadString(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlSource);
                var htmlNodes = doc.DocumentNode.SelectNodes("//a[@href]").Select(node => node.Attributes["href"].Value).ToList();                
                foreach(var node in htmlNodes)
                {
                    text += node.ToString()+" ";
                }
                return text;         
            }catch(Exception e)
            {
                ViewBag.kontrol = false;
                ViewBag.hata = e.ToString();
            } 
            return text;                     
        }

        public IActionResult Asama5(String url)
        {   
            Asama5 p = new Asama5();
            List<string> temp = kelimeleriBul(url);
            foreach (var grp in temp.GroupBy(i => i))
            {  
                p.text.Add(new Tuple<string,int>(grp.Key, grp.Count()));
            }
            p.text.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            ViewBag.index = esanlamliBul(p);     
            return View(p);
        }
        public IActionResult Index(String url)
        {   
           return View();
        }
        private List<string> kelimeleriBul(String url)
        {
            List<string> temp = new List<string>();
            if ((url != null))
            {
                if(url.Length<8)
                {
                    url = @"https:\\"+ url;
                }
                if(!url.Substring(0,8).Equals("https://"))
                {
                    url = @"https:\\"+ url;
                }
                try
                {
                    WebRequest istek = HttpWebRequest.Create(url);                
                    WebResponse cevap;
                    cevap = istek.GetResponse();                    
                    StreamReader donenBilgiler = new StreamReader(cevap.GetResponseStream());
                    string gelen = donenBilgiler.ReadToEnd();   
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(gelen);
                    var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//body");
                    string text="";
                    foreach(var node in htmlNodes)
                    {
                        text = node.InnerText;
                    }
                    String[] split = text.Split(' ', ',', '.', '!', '-');
                    foreach(string a in split)
                    {
                        var regex = new Regex(@"\b[\w']+\b");
                        var match = regex.Match(a);
                        int value;
                        if(a.Length>2 && match.Value.Equals(a) == true && !int.TryParse(a,out value))
                        {
                            temp.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a));
                        }
                    }
                    return temp;                    
                }catch(Exception e)
                {
                    ViewBag.kontrol = false;
                    ViewBag.hata = e.ToString();
                }                    
            }
            return temp;
        }
        private int[] esanlamliBul(Asama5 p)
        {            
            int temp = 4;
            int[] index= new int[5];
            int sayac=0;
            for(int i=0; i<p.text.Count; i++)
            {
                bool cntrl =true;
                try{                    
                    WebRequest istek = HttpWebRequest.Create("https://www.thesaurus.com/browse/" + p.text[i].Item1 + "?s=t");                
                    WebResponse cevap;
                    cevap = istek.GetResponse();                    
                    StreamReader donenBilgiler = new StreamReader(cevap.GetResponseStream());
                    string gelen = donenBilgiler.ReadToEnd();   
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(gelen);
                    var links = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"meanings\"]/div[2]/ul");
                    string text = "s j";
                    foreach(var item in links)
                    {
                        text = item.InnerText;
                    }
                    string[] split = text.Split(' ');
                    int k=0;
                    foreach(var a in split)
                    {
                        p.esanlamlilar.Add(a);
                        if(k++ == 2)break;
                    }
                }catch(Exception e)
                {
                    Console.WriteLine("HATA "+i.ToString()+": "+e.ToString());
                    temp++;
                    cntrl = false;
                }
                if(cntrl == true)
                {
                    index[sayac++] = i;
                }
                if(i==temp) break; 
            }
            return index;
        }
        
    }
}