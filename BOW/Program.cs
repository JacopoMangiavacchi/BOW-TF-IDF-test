using System;
using System.Collections.Generic;
using System.Linq;

namespace BOW
{
    public interface ITransform<TIn, TOut>
    {
        TOut Transform(TIn input);
    }

    public interface IMetadata<TOut>
    {
        TOut GetMetadata();
    }

    public class Tokenizer : ITransform<IEnumerable<string>, IEnumerable<IEnumerable<string>>> 
    {
        private IEnumerable<string> Tokenize(string doc)
        {
            return doc.ToLower().Split(new char[] { ' ', ',', '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public IEnumerable<IEnumerable<string>> Transform(IEnumerable<string> input)
        {
            return input.Select(d => Tokenize(d));
        }
    }


    //public class TFIDFBOW : ITransform<TIn, TOut>
    //{

    //}



    class Program
    {
        static string[] dataset = { "Well done! You really made a great job. Really well done!!",
                                    "Good work.I appreciate your effort",
                                    "Great effort. You should continue this way",
                                    "nice work man, that was great",
                                    "Excellent job! vEry well."
                                  };


        static void Main(string[] args)
        {
            var t = new Tokenizer();
            var tokens = t.Transform(dataset);



            var bow = CreateTFIDFBOW(dataset);




        }

        static string[] Tokenize(string doc)
        {
            return doc.ToLower().Split(new char[] { ' ', ',', '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries);
        }

        static Dictionary<string, (int, Dictionary<int, int>)> CreateTFIDFBOW(string[] corpus)
        {
            var bow = new Dictionary<string, (int, Dictionary<int, int>)>();  // token : (freq in dataset, (doc, [freq in doc]))

            var docId = 0;
            foreach (var doc in corpus)
            {
                var wordsAlreadyInDoc = new List<string>();

                foreach (var word in Tokenize(doc))
                {
                    if (bow.ContainsKey(word))
                    {
                        var x = bow[word];
                        x.Item1++;
                        if (wordsAlreadyInDoc.Contains(word))
                        {
                            x.Item2[docId] += 1;
                        }
                        else
                        {
                            x.Item2[docId] = 1;
                        }
                        bow[word] = x;
                    }
                    else
                    {
                        bow[word] = (1, new Dictionary<int, int>() { { docId, 1 } });
                    }


                    wordsAlreadyInDoc.Add(word);
                }

                docId++;
            }

            dumpInternalBow(bow);

            return bow;
        }

        static void dumpInternalBow(Dictionary<string, (int, Dictionary<int, int>)> bow)
        {
            foreach (KeyValuePair<string, (int, Dictionary<int, int>)> kvp in bow)
            {
                Console.Write($"{kvp.Key} : freqInDataset={kvp.Value.Item1} ");

                foreach (KeyValuePair<int, int> kvp2 in kvp.Value.Item2)
                {
                    Console.Write($"freqInDocs{kvp2.Key}={kvp2.Value} ");
                }

                Console.WriteLine();
            }
        }
    }
}
