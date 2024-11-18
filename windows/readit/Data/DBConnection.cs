using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework_DB;
using EntityFramework_DB.Context;
using Microsoft.Extensions.Configuration;
using readit.Controls;
using ef = EntityFramework_DB;

namespace readit.Data
{
    public class DBConnection
    {
        private readonly ReaditContext context = new ReaditContext();

        private ControlLogs clog = new ControlLogs();

        public void RealizarConexaoDB()
        {
            try
            {
                ef.Global.ConnectionString = App.config.GetConnectionString("ConnectionString");
            }
            catch(Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "RealizarConexaoDB()");
            }
        }

        public bool TestarConexaoDB()
        {
            try
            {
                return context.Database.CanConnect();
            }
            catch(Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "TestarConexaoDB()");
                return false;
            }
        }
    }
}
