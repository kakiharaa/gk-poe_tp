﻿using System;
using System.Collections.Generic;

namespace SiteMVC
{
    public partial class Categorie
    {
        public Categorie()
        {
            Produit = new HashSet<Produit>();
        }

        public int Id { get; set; }
        public string Categorie1 { get; set; }

        public virtual ICollection<Produit> Produit { get; set; }
    }
}
