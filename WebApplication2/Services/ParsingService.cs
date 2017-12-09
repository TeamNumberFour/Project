using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet;
using WebApplication2.Models;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.IO;
using GoogleApi;
using VkNet.Utils;
using System.Threading;

namespace WebApplication2.Services
{
    public class ParsingService: IParsingService
    {
        private  string Query;
        private List<Post> Com=new List<Post>();
        public async Task<Post[]> Common(string query, string Uni, string Fac)

        {
            Query =query;
            await Gazeta(DateTime.Now.AddDays(-21).Date.ToString("dd.MM.yy"), "");
            await vesti();
            await regnum();
            await prof(Uni, Fac); 
            
            
            await VK(100, 21, 0);



            return Com.ToArray();
            



        }
        public async Task  VK(long Count, int daysAgo, int daysTo)
        {
            Random rand = new Random();
           
            VkApi vk = new VkApi();
            Post[] posts;
            var unixTimestamp = DateTime.Now.AddDays(-daysAgo);
            var unixTimestamp2 = DateTime.Now.AddDays(-daysTo);


            ulong appID = 6267940;
            string email = "89234255908";
            string password = "TeamFour";

            if (!vk.IsAuthorized)
                vk.Authorize(new ApiAuthParams
                {
                    ApplicationId = appID,
                    Login = email,
                    Password = password
                });

            var search = vk.NewsFeed.Search(new VkNet.Model.RequestParams.NewsFeedSearchParams
            {
                Query = Query,
                Count = Count,
                StartTime = unixTimestamp,
                EndTime = unixTimestamp2
            });

            posts = new Post[search.Count];

            for (int i = 0; i < search.Count; i++)
            {
                Post post = new Post();
                /*if (search[i].Comments.Count != 0)
                {
                    var getComments = vk.Wall.GetComments(new VkNet.Model.RequestParams.WallGetCommentsParams
                    {
                        PostId = search[i].Id,
                        OwnerId = search[i].OwnerId,
                        NeedLikes = true,
                        Extended = true
                    });

                    if (search[i].Comments.Count != 0)
                    {
                        Comment[] comments = new Comment[getComments.Count];
                        for (int j = 0; j < getComments.Count; j++)
                        {
                            Comment comment = new Comment();
                            comment.text = getComments[j].Text;

                            var getName = vk.Users.Get(Math.Abs(search[i].FromId));
                            comment.ownersName = getName.FirstName + " " + getName.LastName;

                            comments[j] = comment;
                        }
                        post.comments = comments;
                    }

                }*/
                post.text = search[i].Text;
                var getNameForPost = vk.Users.Get(Math.Abs(search[i].OwnerId));
                post.ownersName = getNameForPost.FirstName + " " + getNameForPost.LastName;
                post.link = "https://vk.com/wall" + search[i].OwnerId + "_" + search[i].Id;
                post.date = search[i].Date.ToString();
                post.emo = rand.Next(3);
                post.source = "vk";
                post.pass = true;
                posts[i] = post;
                Thread.Sleep(1);
            }
            Com.AddRange(posts);
        }
        // профессор рейтинг
        public async Task prof(string uni, string fac)
        {
            Random rand = new Random();
            var str = "";
            var newUrl = "";
            string url = "";
            List<Post> posts = new List<Post>();

            using (HttpClient client = new HttpClient())
            {
                if (fac != "")
                    url = "https://yandex.ru/search/?text=" + fac + "%20" + uni + "%20professorrating.org&lr=67";
                else
                {
                    url = "https://yandex.ru/search/?text=" + "Отзывы" + "%20" + uni + "%20professorrating.org&lr=67";
                }
                var htmlCode = await client.GetStringAsync(url);
                Task.WaitAll();
                str = htmlCode;
            }


            string reg1;
            if (fac == "")
                reg1 = @"(professorrating.org/university.php.id=\d{0,7})";
            else
                reg1 = @"(professorrating.org/faculty.php.id=\d{0,10})";

            Regex rgx1 = new Regex(reg1);
            newUrl = "https://" + rgx1.Match(str).Groups[1].Value;

            using (HttpClient client = new HttpClient())
            {

                string htmlCode = await client.GetStringAsync(newUrl);
                Task.WaitAll();
                str = htmlCode;
            }

            string reg2 = @"user-name..(\n[ ]{32,35})?([\s\S]{0,20})([ ]{28,50})?\<a rel.{40,60}href..(.{7,15}).\>#[\s\S]{10,300}date.comment.\>(.{2})(.nbsp.|[ ]{0,1})(.{3,10}).nbsp.(.{4}).{0,20}p[\s\S]{500,650}comment..(\n[ ]{30,50})?([\s\S]{0,1000})([ ]{30,50})?\</p\>\n([ ]{0,60}\n)?\<form method..post";
            Regex rgx2 = new Regex(reg2);
            DateTime from = DateTime.Now.AddDays(-21);
            DateTime to = DateTime.Now;
            foreach (Match match in rgx2.Matches(str))
            {
                Post post = new Post();
                var Date = Convert.ToDateTime(match.Groups[5].Value + " " + match.Groups[7].Value + " " + match.Groups[8].Value);

                if (Date.CompareTo(from) > 0 && Date.CompareTo(to) < 0)
                {
                    post.date = Date.ToShortDateString();
                    post.link = newUrl + match.Groups[4].Value;
                    post.text = match.Groups[10].Value;
                    post.ownersName = match.Groups[2].Value;
                    post.emo = rand.Next(3);
                    post.source = "prof";
                    post.pass = true;
                    Thread.Sleep(1);
                    posts.Add(post);
                }
            }
            Com.AddRange(posts);
        }
        // вести
        public async Task vesti()
        {
            Random rand = new Random();
            var str = "";

            DateTime from = DateTime.Now.AddDays(-21);
            DateTime to = DateTime.Now;
            List<Post> posts = new List<Post>();
            using (HttpClient client = new HttpClient())
            {

                string url = "http://www.vesti.ru/search/index/?q=" + Query + "&news=0&video=0&date_start=" + from + "&date_end=" + to;
                string htmlCode = await client.GetStringAsync(url);
                str = htmlCode;
            }

            string pattern = @"search-item__date__day.\>(.{10,20})\<.p[\s\S]{0,150}(\<a href[\s\S]{30,100}\</a\>[\s\S]{10,50})?href=.(.{10,20}).\>(([\s\S]{0,100}).span class.{0,50}span.)?([\s\S]{0,200})\</a\>[\s\S]{0,10}\</h3\>";
            Regex rgx = new Regex(pattern);
            foreach (Match match in rgx.Matches(str))
            {
                Post post = new Post();

                post.date = match.Groups[1].Value;
                post.link = "http://www.vesti.ru" + match.Groups[3].Value;
                if (match.Groups[4].Value == "")
                    post.text = match.Groups[6].Value;
                else
                    post.text = match.Groups[5].Value + Query + match.Groups[6].Value;
                post.ownersName = "noname";
                post.emo = rand.Next(3);
                post.source = "vesti";
                post.pass = true;
                Thread.Sleep(1);
                posts.Add(post);
            }
            Com.AddRange(posts);
        }

        //  регнум

        public async Task regnum()
        {
            Random rand = new Random();
            var str = "";

            List<Post> posts = new List<Post>();
            using (HttpClient client = new HttpClient())
            {

                string url = "https://regnum.ru/news/search/" + Query + ".html";
                var result = await client.GetStringAsync(url);
                Task.WaitAll();
                str = result.ToString();
            }
            string pattern = @"\<a href=.(.{40,50}). class=.news-container[\s\S]{200,420}data-location.{40,50}\>[\s\S]{17}(.{8})[\s\S]{327}([\s\S]{0,200})[ ]{16}\</span";
            Regex rgx = new Regex(pattern);
            DateTime from = DateTime.Now.AddDays(-21);
            DateTime to = DateTime.Now;
            foreach (Match match in rgx.Matches(str))
            {
                Post post = new Post();
                var Date = Convert.ToDateTime(match.Groups[2].Value);
                if (Date.CompareTo(from) > 0 && Date.CompareTo(to) < 0)
                {
                    post.date = Date.ToShortDateString();
                    post.link = match.Groups[1].Value;
                    post.text = match.Groups[3].Value;
                    post.ownersName = "noname";
                    post.emo = rand.Next(3);
                    post.source = "regnum";
                    post.pass = true;
                    Thread.Sleep(1);

                    posts.Add(post);
                }
            }
            Com.AddRange(posts);
        }

    public async Task Gazeta( string from, string to)
        {
            Random rand = new Random();
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
                post.emo = rand.Next(3);
                post.source = "gazeta";
                post.pass = true;
                Thread.Sleep(1);
                posts.Add(post);
            }

            Com.AddRange(posts);
        }
    }
    }

