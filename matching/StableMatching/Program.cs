using System;
using System.ComponentModel.Design;
using System.Transactions;

namespace Matching
{
    class Man
    {
        private string _name;
        private List<int> _preferences = new List<int>();
        private int _next = 0;
        private int _id;

        public Man(string name, int id)
        {
            _name = name;
            _id = id;
        }

        public void AddPreference(int id)
        {
            _preferences.Add(id);
        }

        public string GetName()
        {
            return _name;
        }

        public int GetNext()
        {
            return _preferences[_next];
        }

        public void IncrementNext()
        {
            _next++;
        }

        public int GetId()
        {
            return _id;
        }

        public int GetCurrent()
        {
            return _preferences[_next - 1];
        }
    }

    class Woman
    {
        private string _name;
        private int[] _preferences;
        private int _matchedWith = -1;
        private int _id;
        private int _p;

        public Woman(string name, int id, int men)
        {
            _name = name;
            _id = id;
            _preferences = new int[men];
            _p = men;
        }

        public void SetPreference(int id)
        {
            _preferences[id] = _p--;
        }

        public string GetName()
        {
            return _name;
        }

        public bool IsMatched()
        {
            return _matchedWith != -1;
        }

        public void MatchWith(int id)
        {
            _matchedWith = id;
        }

        public int GetId()
        {
            return _id;
        }

        public int GetMatched()
        {
            return _matchedWith;
        }

        public bool Prefers(int id)
        {
            return _preferences[id] > _preferences[_matchedWith];
        }
    }

    public class Matching
    {
        private static int IdToIndex(int id)
        {
            return (id - 1) / 2;
        }

        private static void AddManPreferences(List<Man> men, int id, string[] words)
        {
            int index = IdToIndex(id);
            Man man = men[index];
            for (int j = 1; j < words.Length; j++)
            {
                if (words[j].Length == 0)
                    continue;
                int preference = IdToIndex(int.Parse(words[j]));
                man.AddPreference(preference);
            }
        }

        private static void AddWomanPreferences(List<Woman> women, int id, string[] words)
        {
            int index = IdToIndex(id);
            Woman woman = women[index];
            for (int j = 1; j < words.Length; j++)
            {
                if (words[j].Length == 0)
                    continue;
                int manId = IdToIndex(int.Parse(words[j]));
                woman.SetPreference(manId);
            }
        }

        private static (List<Man>, List<Woman>) ReadInput()
        {
            string line;
            do
            {
                line = Console.ReadLine();
            } while (line[0] == '#');

            int n = int.Parse(line.Split("=")[1]);

            List<Man> men = new List<Man>();
            List<Woman> women = new List<Woman>();

            for (int i = 0; i < n; i++)
            {
                string man = Console.ReadLine().Split()[1];
                men.Add(new Man(man, i));
                string woman = Console.ReadLine().Split()[1];
                women.Add(new Woman(woman, i, n));
            }

            Console.ReadLine();

            for (int i = 0; i < n; i++)
            {
                string[] words = Console.ReadLine().Split();
                int id = int.Parse(words[0].Substring(0, words[0].Length - 1));
                if (id % 2 == 1)
                    AddManPreferences(men, id, words);
                else
                    AddWomanPreferences(women, id, words);

                words = Console.ReadLine().Split();
                id = int.Parse(words[0].Substring(0, words[0].Length - 1));
                if (id % 2 == 1)
                    AddManPreferences(men, id, words);
                else
                    AddWomanPreferences(women, id, words);
            }
            return (men, women);
        }

        private static void StableMatching(List<Man> men, List<Woman> women)
        {
            Stack<Man> free = new Stack<Man>();
            men.ForEach(man => free.Push(man));
            while (free.Count != 0)
            {
                Man man = free.Pop();
                int next = man.GetNext();
                Woman woman = women[next];
                man.IncrementNext();
                if (!woman.IsMatched())
                    woman.MatchWith(man.GetId());
                else if (woman.Prefers(man.GetId()))
                {
                    Man rejected = men[woman.GetMatched()];
                    free.Push(rejected);
                    woman.MatchWith(man.GetId());
                }
                else
                    free.Push(man);
            }
        }

        private static void PrintMatching(List<Man> men, List<Woman> women)
        {
            foreach (Man man in men)
                Console.WriteLine($"{man.GetName()} -- {women[man.GetCurrent()].GetName()}");
        }

        public static void Main(string[] args)
        {
            var (men, women) = ReadInput();
            StableMatching(men, women);
            PrintMatching(men, women);
        }
    }
}
