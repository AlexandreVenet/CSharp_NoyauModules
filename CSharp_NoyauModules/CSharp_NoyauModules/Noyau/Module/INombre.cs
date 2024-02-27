using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_NoyauModules.Noyau.Module
{
	internal interface INombre
	{
		// On utilise nullable pour que le module puisse déterminer si on dispose bien d'une valeur
		int? ObtenirNombre();
		void GenererNombre();
	}
}
