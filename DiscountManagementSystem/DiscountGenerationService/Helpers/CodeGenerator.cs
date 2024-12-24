namespace DiscountGenerationService.Helpers
{
    public class CodeGenerator
    {
        private string _storage;
        private int _slots;
        private string _code_alphabet;

        public CodeGenerator(string code_alphabet, string storage, int slots)
        {
            _storage = storage;
            _slots = slots;
            _code_alphabet = code_alphabet;
        }

        public void CreateRandomSlots(string prefixes)
        {
            Random rng = new Random();

            string[] slots_and_state = new string[_slots + 1];

            slots_and_state[0] = prefixes;
            for (int i = 1; i < _slots; i++)
            {
                slots_and_state[i] = new string(_code_alphabet.OrderBy(_ => rng.Next()).ToArray());
            }
            slots_and_state[_slots] = string.Join("|", Enumerable.Repeat("0", _slots));

            if (!File.Exists(_storage))
            {
                File.WriteAllLines(_storage, slots_and_state);
            }
        }

        public IEnumerable<string> GenerateDiscountCodes(int n_codes)
        {
            // Read file content
            string[] slots_and_state = File.ReadAllLines(_storage);

            string prefixes = new string(slots_and_state.First());
            string[] slots = slots_and_state.Take(slots_and_state.Length - 1).ToArray();
            int[] state = slots_and_state.Last().Split("|").Select(x => int.Parse(x)).ToArray();

            for (int i = 0; i < n_codes; i++)
            {
                bool success = false;

                slots_and_state[slots_and_state.Length - 1] = string.Join("|", state.Select(x => x.ToString()).ToArray());

                while (!success)
                {
                    try
                    {                      
                        success = true;
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"File write error: {ex.Message}. Retrying...");
                        System.Threading.Thread.Sleep(100);
                    }
                }

                yield return Stringify(state, slots); // Return the character

                state = Increment(state, prefixes.Length - 1, _code_alphabet.Length - 1);
            }

            File.WriteAllLines(_storage, slots_and_state);
        }

        private static int[] Increment(int[] state, int pref_len, int max_len)
        {
            for (int slot = state.Length - 1; slot >= 0; --slot)
            {
                if (slot == 0 && state[slot] == pref_len)
                {
                    throw new Exception("Exhausted!");
                }

                state[slot] += 1;
                
                if (state[slot] > max_len)
                {
                    state[slot] = 0;
                    continue;
                }
                else
                {
                    break;
                }              
            }
            return state;
        }

        private static string Stringify(int[] state, string[] slots) 
            => new string(state.Select((value, slot) => slots[slot][value]).ToArray());
    }
}
