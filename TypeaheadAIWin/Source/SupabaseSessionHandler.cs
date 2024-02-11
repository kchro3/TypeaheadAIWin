using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TypeaheadAIWin.Source
{
    public class SupabaseSessionHandler : IGotrueSessionPersistence<Session>
    {
        void IGotrueSessionPersistence<Session>.DestroySession()
        {
            Trace.WriteLine("Destroy");
            Properties.Settings.Default.Session = null;
            Properties.Settings.Default.Save();
        }

        Session? IGotrueSessionPersistence<Session>.LoadSession()
        {
            Trace.WriteLine("Load");
            if (Properties.Settings.Default.Session != null)
            {
                return JsonSerializer.Deserialize<Session>(Properties.Settings.Default.Session);
            } 
            else
            {
                return null;
            }
        }

        void IGotrueSessionPersistence<Session>.SaveSession(Session session)
        {
            Trace.WriteLine("Save");
            Properties.Settings.Default.Session = JsonSerializer.Serialize(session);
            Properties.Settings.Default.Save();
        }
    }
}
