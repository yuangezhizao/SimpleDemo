using System;
using Noesis.Javascript;

namespace Commons
{
    public class JsContext
    {
        public static string Responsejs(string script, string key)
        {
            using (JavascriptContext context = new JavascriptContext())
            {
                try
                {
                    context.Run(script);

                    var res = context.GetParameter(key);
                    return res.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;

                }
             
            }
        }
    }
}
