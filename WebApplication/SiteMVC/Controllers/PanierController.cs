﻿using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SiteMVC.Models.ViewModels;
using SiteMVC.Repositories;
using System.Collections.Generic;

namespace SiteMVC.Controllers {
    public class PanierController : Controller {
        IRepository<Produit> _produitRepository;           //Possible de passer le modele à AddToCart

        public PanierController(IRepository<Produit> produitRepository) {
            _produitRepository = produitRepository;
        }

        // TODO : vue partielle ou modale pour choix quantite ?
        //[HttpGet]
        //public ActionResult AddToCart(int id) {        // ajouter la quantite a Details, voire creer une vue intermediaire
        //    
        //    return RedirectToAction("Index", "Tirelires");      // Retour à la page gallerie
        //}

        [HttpPost]
        public ActionResult AddToCart(int id, int quantite) {
            string currentCartSerialized = HttpContext.Session.GetString("Cart");
            Dictionary<int, int> currentCart;
            if (currentCartSerialized.IsNullOrEmpty()) {
                currentCart = new Dictionary<int, int>();
                currentCart[id] = quantite;
            }
            else {
                currentCart = JsonConvert.DeserializeObject<Dictionary<int, int>>(currentCartSerialized);
                if (currentCart.ContainsKey(id)) {
                    currentCart[id] += quantite;
                }
                else {
                    currentCart[id] = quantite;
                }
            }
            currentCartSerialized = JsonConvert.SerializeObject(currentCart);
            HttpContext.Session.SetString("Cart", currentCartSerialized);
            return RedirectToAction("Index", "Tirelires");
        }
        // GET: PanierController
        public ActionResult SeeCart() {
            string currentCartSerialized = HttpContext.Session.GetString("Cart");
            Dictionary<int, int> currentCart;
            if (currentCartSerialized.IsNullOrEmpty()) {
                return RedirectToAction("Index", "Tirelires");
            }
            else {
                currentCart = JsonConvert.DeserializeObject<Dictionary<int, int>>(currentCartSerialized);
            }
            List<Produit> listeProduits = new List<Produit>();
            foreach (KeyValuePair<int, int> infosProduit in currentCart) {
                // TODO : mettre en cache cette liste pour l'utiliser dans le CommandesController
                Produit produit = _produitRepository.GetById(infosProduit.Key);
                listeProduits.Add(produit);
                // Pour associer la quantité au produit
                ViewData[produit.Id.ToString()] = infosProduit.Value;
            }
            return View(listeProduits);
        }


        //// GET: PanierController/Details/5
        //public ActionResult Details(int id) {
        //    return View();
        //}


        //// GET: PanierController/Edit/5
        //public ActionResult Edit(int id) {
        //    return View();
        //}


    }
}
