using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet;
using WebApplication2.Models;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.IO;





namespace WebApplication2.Services
{
    public class ParsingService: IParsingService
    {
        private  string Query;
        private List<Post> Com=new List<Post>();
        public async Task<Post[]> Common(string query)

        {
            Query =query;
           await Gazeta(DateTime.Now.AddDays(-21).Date.ToString("dd.MM.yy"), "");
            

            return Com.ToArray();
            



        }
       public async Task Gazeta( string from, string to)
        {
            var str = "";

            List<Post> posts = new List<Post>();
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetStringAsync("https://www.gazeta.ru/search.shtml?p=search&page=0&text=" + Query + "&article=&section=&from=" + from + "&to=" + to + "&sort_order=published_desc&input=utf8");
                Task.WaitAll();
                str = result.ToString();
            
            }
            
            string pattern = @"pubdate=.(.{10})[\s\S]{0,100} 5px;..\n\<a href=.(/\w+/news/.{0,50}shtml)[\s\S]{0,20}\>\n([\s\S]{0,300})\n\</a\>\n\</h2>([\s\S]{0,4000} s_author_name..(.{0,50})..span)?";
            Regex rgx = new Regex(pattern);

            foreach (Match match in rgx.Matches(str))
            {
                Post post = new Post();

                post.date = match.Groups[1].Value;
                post.link = "https://www.gazeta.ru/" + match.Groups[2].Value;
                post.text = match.Groups[3].Value;

                if (match.Groups[4].Value != "")
                    post.ownersName = match.Groups[5].Value;
                else
                    post.ownersName = "noname";

                posts.Add(post);
            }

            Com.AddRange(posts);
        }
    }
    }

