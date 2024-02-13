using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using System.Diagnostics;
using System.Text.Json;

namespace TypeaheadAIWin.Source
{
    public class SupabaseSessionHandler : IGotrueSessionPersistence<Session>
    {
        void IGotrueSessionPersistence<Session>.DestroySession()
        {
            Properties.Settings.Default.Session = null;
            Properties.Settings.Default.Save();
        }

        Session? IGotrueSessionPersistence<Session>.LoadSession()
        {
            try
            {
                if (Properties.Settings.Default.Session != null)
                {
                    return JsonSerializer.Deserialize<Session>(Properties.Settings.Default.Session);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to load session\n" + ex);
            }

            return null;
        }

        void IGotrueSessionPersistence<Session>.SaveSession(Session session)
        {
            Properties.Settings.Default.Session = JsonSerializer.Serialize(session);
            Properties.Settings.Default.Save();
        }
    }
}
