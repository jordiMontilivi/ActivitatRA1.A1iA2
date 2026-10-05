using System;
using System.Collections.Generic;
using System.Text;

namespace NetFlixApp.DADES
{
    public class DAOFactory
    {
        public static IDAO CreateDAO()
        {
            return new DAOCsvImpl();
        }
    }
}
