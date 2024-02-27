using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_NoyauModules.Noyau.Module
{
	internal interface IDonnees
	{
		void ChercherDonneesQuelquePart();
		Type ObtenirTypeDonnees();
		object ObtenirDonnees();
	}
}
