using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _15.Controllers
{
    public class SolveController : ApiController
    {
        // GET: api/Solve
        public IEnumerable<string> Get(int mx, string jnum)
        {
            var num = JsonConvert.DeserializeObject<IList<int>>(jnum);
            return Solve(mx, num);
        }

        private IList<string> Solve(int mx, IList<int> num)
        {
            var visited = new Dictionary<string, bool>();
            var path = new Dictionary<string, IList<string>>();

            var dn = EncodeNum(num);
            visited[dn] = true;
            var queue = new List<string> { dn };

            while (queue.Count() > 0)
            {
                var odn = queue[0];
                queue.RemoveAt(0);
                num = DecodeNum(odn).ToList();
                var mvs = GetMovable(mx, num);
                foreach (var mv in mvs)
                {
                    num = DecodeNum(odn).ToList();
                    Move(mx, num, mv);
                    dn = EncodeNum(num);
                    if (IsSuccess(num))
                    {   // 見つかった！
                        var res = new List<string> { odn, dn };
                        Result(res, odn, path);
                        return res;
                    }
                    if (!visited.ContainsKey(dn))
                    {
                        visited[dn] = true;
                        queue.Add(dn);
                        if (!path.ContainsKey(odn))
                            path[odn] = new List<string>();
                        path[odn].Add(dn);
                    }
                }
            }
            return null;
        }

        private void Result(IList<string> res, string dn, IDictionary<string, IList<string>> path)
        {
            foreach (var a in path.Keys)
                if (path[a].Where(t => t == dn).FirstOrDefault() != null)
                {
                    res.Insert(0, a);
                    Result(res, a, path);
                }
        }


        private string EncodeNum(IList<int> num)
        {
            return string.Join(",", num.Select(t => t.ToString()));
        }

        private IEnumerable<int> DecodeNum(string str)
        {
            return str.Split(',').Select(t => int.Parse(t));
        }

        private void Swap(IList<int> num, int c, int n)
        {
            var tmp = num[c];
            num[c] = num[n];
            num[n] = tmp;
        }

        private int Move(int mx, IList<int> num, int id)
        {
            var max = num.Count();
            if (id - mx >= 0 && num[id - mx] == max - 1)
            {
                Swap(num, id, id - mx);
                return id - mx;
            }
            else if (id + mx<max && num[id + mx] == max - 1)
            {
                Swap(num, id, id + mx);
                return id + mx;
            }
            else if (id % mx - 1 >= 0 && num[id - 1] == max - 1)
            {
                Swap(num, id, id - 1);
                return id - 1;
            }
            else if (id % mx + 1 < mx && num[id + 1] == max - 1)
            {
                Swap(num, id, id + 1);
                return id + 1;
            }
            return id;
        }
        
        private bool IsSuccess(IList<int> num)
        {
            for (var i = 0; i < num.Count(); i++)
                if (num[i] != i)
                    return false;
            return true;
        }

        private int GetBlank(IList<int> num)
        {
            return num.IndexOf(num.Count() - 1);
        }

        private IEnumerable<int> GetMovable(int mx, IList<int> num)
        {
            var id = GetBlank(num);
            if (id - mx >= 0)
                yield return id - mx;
            if (id + mx < num.Count())
                yield return id + mx;
            if (id % mx - 1 >= 0)
                yield return id - 1;
            if (id % mx + 1 < mx)
                yield return id + 1;
        }
    }
}
