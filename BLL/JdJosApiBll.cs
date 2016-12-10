using System;
using DataBase;
using Mode;

namespace BLL
{
    public class JdJosApiBll
    {
        public void AddJosApi(JdJosApi jos)
        {
            jos.CreateTime=DateTime.Now;
            new JdJosApiDB().AddJosApi(jos);
        }

        public JdJosApi GetJosApi(int id)
        {
            return new JdJosApiDB().GetJosApi(id);
        }

    }
}
