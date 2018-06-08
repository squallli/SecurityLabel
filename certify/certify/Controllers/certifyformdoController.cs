using Aitag.Models;
using certify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace certify.Controllers
{
    public class certifyformdoController : Controller
    {
        private AitagDBContext _db;
        public certifyformdoController()
        {
            _db = new AitagDBContext();
        }

        public ActionResult Index(string barcode)
        {
            certifystampstock cer =  _db.Certifystampstock.Where( e=>e.barcode == barcode && e.ifrand == "n").SingleOrDefault();
            looknumber look = _db.lookNumber.Where(e => e.barcode == barcode).SingleOrDefault();


            string item = "";

            if (cer == null)
            {
                return RedirectToAction("certifynon", "certify");
            }
            else
            {
                if(look != null)
                {
                    look.lookno += 1;
                    look.bmoddate = DateTime.Now;
                    _db.Entry(look).State = System.Data.Entity.EntityState.Modified;

                    _db.SaveChanges();
                }
                else
                {
                    looknumber _looknumber = new looknumber()
                    {
                        barcode = barcode,
                        lookno = 1,
                        bmoddate = DateTime.Now
                    };

                    _db.lookNumber.Add(_looknumber);
                    _db.SaveChanges();
                }

            }
            return RedirectToAction("certification", "certify", new { @barcode = barcode, @randno = item, @codno = cer.codno, @codid = cer.codid });

        }

        
    }
}