using Aitag.Models;
using certify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace certify.Controllers
{
    public class certifyController : Controller
    {
        private AitagDBContext _db;
        public certifyController()
        {
            _db = new AitagDBContext();
        }
        // GET: certify
        public ActionResult Index()
        {
            return View();
        }

       

        public ActionResult certification(string barcode,string randno,string codno,string codid)
        {

            try
            {
                ViewBag.barcode = barcode;

                looknumber look = _db.lookNumber.Where(e => e.barcode == barcode).SingleOrDefault();
                certifyorder certiFyorder = _db.certifYorder.Where(e => e.codno == codno).SingleOrDefault();
                List<viewcertifycheckdet> viewCertifycheckdet = _db.viewCertifycheckdet.Where(e => e.farmerno == certiFyorder.farmerno).ToList();
                farmer Farmer = _db.Farmer.Where(e => e.farmerno == certiFyorder.farmerno).SingleOrDefault();
                barcodeRule rule = _db.BarcodeRule.Where(e => e.barcode == barcode).SingleOrDefault();
                ViewBag.applicant = Farmer.farmername;
                ViewBag.faddr = Farmer.faddr;
                ViewBag.position = rule.positionRule + 1;

                Farmer = _db.Farmer.Where(e => e.farmerno == certiFyorder.vendno).SingleOrDefault();
                ViewBag.vendor = Farmer.farmername;

                certificationViewModel certificationViewM = new Models.certificationViewModel();
                certificationViewM.certifYorder = certiFyorder;
                certificationViewM.Farmer = Farmer;
                certificationViewM.lookNumber = look;
                certificationViewM.viewCertifycheckdet = viewCertifycheckdet;

                return View(certificationViewM);
            }
            catch
            {
                return RedirectToAction("certifynon");
            }
            
        }

        public ActionResult certifynon()
        {
            return View();
        }

        public ActionResult certifyformdo(string barcode)
        {
            certifystampstock cer = _db.Certifystampstock.Where(e => e.barcode == barcode && e.ifrand == "n").SingleOrDefault();
            looknumber look = _db.lookNumber.Where(e => e.barcode == barcode).SingleOrDefault();


            string item = "";

            if (cer == null)
            {
                return RedirectToAction("certifynon", "certify");
            }
            else
            {
                if (look != null)
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