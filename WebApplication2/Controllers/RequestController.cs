using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication2.Models;
using WebApplication2.Models.Requests;
using WebApplication2.Data;
using WebApplication2.Models.AccountViewModels;
using WebApplication2.Services;
using Microsoft.EntityFrameworkCore;
using VkNet.Utils;
namespace WebApplication2.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly string _externalCookieScheme;
        private readonly ApplicationDbContext context;
        private readonly IParsingService parsingService;

        public RequestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            ApplicationDbContext context,
            IParsingService parsingService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            this.context = context;
            this.parsingService = parsingService;
        }



        
        public async Task<IActionResult> Choose()
        {
            
            var Univer = await this.context.Universities.ToListAsync();
            var Faculty= await this.context.Faculties.ToListAsync();




            var model = new Choose
            {
                Universities = (ICollection<University>)Univer,
                Faculties = (ICollection<Faculty>)Faculty


            };

            return this.View(model);

            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Choose(Choose model, string un, string fa, bool locker)
        {
            if (!locker)
            {
                var Univer = await this.context.Universities.ToListAsync();
                var Faculty = await this.context.Faculties.ToListAsync();
                var model1 = new Choose
                {
                    Universities = (ICollection<University>)Univer,
                    Faculties = (ICollection<Faculty>)Faculty,
                    University = un


                };
                this.ViewBag.University = un;
                return this.View(model1);
            }
            
            Post[] posts =await parsingService.Common(un + " " + fa, un, fa);
            
                var itemsToDelete = context.Set<Post>();
                context.Posts.RemoveRange(itemsToDelete);
                await context.SaveChangesAsync();
            
            foreach (var postitem in posts)
            {
                var post = new Post
                {
                    ownersName=postitem.ownersName,
                    text=postitem.text,
                    date=postitem.date,
                    comments=postitem.comments,
                    link=postitem.link,
                    emo=postitem.emo,
                    pass=postitem.pass,
                    source=postitem.source
                };
                context.Posts.Add(post);
            }
            await this.context.SaveChangesAsync();

            var Postmass = await this.context.Posts.ToListAsync();
            return this.View("List", new List
                 {
                     
                     Posts=(ICollection<Post>)Postmass,
                     
                 });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filtered(List model)
        {
            List<Post> Filt = new List<Post>();
            var Poststofilter = await this.context.Posts.ToListAsync() ;
            foreach (var item in Poststofilter)
            {
                if(model.positive || model.negative || model.neutral || model.uncertain ) switch (item.emo)
                {
                    case 0:
                        if (!model.positive) item.pass = false;
                        break;
                    case 1:
                        if (!model.negative) item.pass = false;
                        break;
                    case 2:
                        if (!model.neutral) item.pass = false;
                        break;
                    case 3:
                        if (!model.uncertain) item.pass = false;
                        break;
                }
                
                if (!item.source.Equals("regnum") &&model.data1>=DateTime.Now.AddDays(-21) && model.data2 <= DateTime.Now) if (DateTime.Parse(item.date) < model.data1 || DateTime.Parse(item.date)> model.data2) item.pass = false;
                if(model.nickname!=null)if (string.Compare(item.ownersName,model.nickname)!=0) item.pass = false;
                if (model.key != null) if (!item.text.Contains(model.key)) item.pass = false;
                List<string> check = new List<string>();
                if (model.abitur) { check.Add("vk"); check.Add("prof"); }
                if (model.student) check.Add("vk");
                if (model.employee) { check.Add("regnum"); check.Add("vesti"); }
                if (model.employer) { check.Add("regnum"); check.Add("vesti"); check.Add("vk"); check.Add("prof"); check.Add("gazeta"); }
                if (model.grad) { check.Add("regnum"); check.Add("vesti"); check.Add("vk"); check.Add("prof"); }
                if (!check.Contains(item.source) && check.Count>0) { item.pass = false; }



                if (item.pass) Filt.Add(item);
            }

            return this.View("List", new List
            {
                filtered = true,
                employee=model.employee,
                student=model.student,
                abitur=model.abitur,
                employer=model.employer,
                grad=model.grad,
                nickname=model.nickname,
                key=model.key,
                positive=model.positive,
                negative=model.negative,
                neutral=model.neutral,
                uncertain=model.uncertain,
                data1=model.data1,
                data2=model.data2,
                Posts=(ICollection<Post>)Filt

            });



        }

    }
}
