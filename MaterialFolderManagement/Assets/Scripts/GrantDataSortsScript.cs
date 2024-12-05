using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;

public class GrantDataSortsScript : FileFolderSavingScript
{
    private enum SortingRating : int
    {
        Character = 0,
        Number,
        Symbol
    }

    /// <summary>
    /// Windows�Ƃ͌��\�قȂ铮��������B��ŏC���������B
    /// </summary>
    /// <param name="sortGrantDatas"></param>
    /// <returns></returns>
    protected FileFoldersGrantData[] SortGrantDataWindowsStyle(FileFoldersGrantData[] sortGrantDatas, bool ascendingOrder = true)
    {
        List<(string name, List<int> num, List<string> symbol, int index)> sortFile = new List<(string name, List<int> num, List<string> symbol, int index)>();
        List<(string name, List<int> num, List<string> symbol, int index)> sortFolder = new List<(string name, List<int> num, List<string> symbol, int index)>();

        //�t�@�C���ƃt�H���_�ŕ����ă\�[�g
        //�L����:�ɐ�����/�ɒu���B
        string name;
        List<int> nums;
        List<string> symbols;
        var rgx_num = new Regex(@"\d+");
        var rgx_symbol = new Regex(@"[^\w\s]");//Regex(@"\W")
        for (int i = 0; i < sortGrantDatas.Length; i++)
        {
            name = sortGrantDatas[i].Name;
            nums = new List<int>();
            symbols = new List<string>();
            name = rgx_symbol.Replace(name, s =>
            {
                symbols.Add(s.Value);
                return ":";
            });
            name = rgx_num.Replace(name, n =>
            {
                int value;
                if (!int.TryParse(n.Value, out value))
                {
                    value = int.Parse(n.Value.Normalize(NormalizationForm.FormKC));
                }
                nums.Add(value);
                return "/";
            });
            /*�f�o�b�N�p
            Debug.Log("<color=yellow>"+ sortGrantDatas[i].Name + "</color>");
            foreach (var item in name.ToCharArray())
            {
                Debug.Log("��" + item);
            }
            foreach (var item in nums)
            {
                Debug.Log("��" + item);
            }
            foreach (var item in symbols)
            {
                Debug.Log("�L" + item);
            }
            //*/
            if (sortGrantDatas[i].Extension == FileFoldersGrantData.extension.folder)
            {
                sortFolder.Add((name, nums, symbols, i));
            }
            else
            {
                sortFile.Add((name, nums, symbols, i));
            }
        }

        sortFile = sortFile
            .OrderBy(file => file.name)
            .ThenBy(file => (file.symbol.Count == 0) ? "" : file.symbol[0])
            .ThenBy(file => (file.num.Count == 0) ? 0 : file.num[0])
            .Select(file => file)
            .ToList(); 

        sortFolder = sortFolder
             .OrderBy(file => file.name)
             .ThenBy(file => (file.symbol.Count == 0) ? "" : file.symbol[0])
             .ThenBy(file => (file.num.Count == 0) ? 0 : file.num[0])
             .Select(file => file)
             .ToList();


        /*
        //�]��
        //��>�L��>����>����
        //�L���u:�v �����u/�v
        sortFolder.Sort((x, y) =>
        {

            string xName="";
            foreach (var item in x.name)
            {
                xName += item;
            }
            string yName ="";
            foreach (var item in y.name)
            {
                yName += item;
            }
            //Debug.Log("<color=yellow>" + +"</color>");

            SortingRating xScore = 0;
            SortingRating yScore = 0;
            int numsIndex = -1;
            int symbolsIndex = -1;
            int nameLength = Math.Min(x.name.Length, y.name.Length);
            for (int i = 0; i < nameLength; i++)
            {
                xScore = ChaScore(x.name[i]);
                yScore = ChaScore(y.name[i]);

                if (xScore == yScore)
                {
                    switch (xScore)
                    {
                        case SortingRating.Character:
                            if (x.name[i].Equals(y.name[i])) continue;
                            Debug.Log(yName + "_" + xName + "_return" + x.name[i].CompareTo(y.name[i]));
                            return x.name[i].CompareTo(y.name[i]);

                        case SortingRating.Number:
                            numsIndex++;
                            if (x.num[numsIndex] == y.num[numsIndex]) continue;
                            Debug.Log(yName + "_" + xName + "_��return" + x.num[numsIndex].CompareTo(y.num[numsIndex]));
                            return x.num[numsIndex].CompareTo(y.num[numsIndex]);

                        case SortingRating.Symbol:
                            symbolsIndex++;
                            if (x.symbol[symbolsIndex].Equals(y.symbol[symbolsIndex])) continue;
                            Debug.Log(xName + "_" + yName + "_��return" + x.symbol[symbolsIndex].CompareTo(y.symbol[symbolsIndex]));
                            return x.symbol[symbolsIndex].CompareTo(y.symbol[symbolsIndex]);
                       
                    }
                }
                else if ((int)xScore > (int)yScore)
                {
                    Debug.Log(xName + "_" + yName + "_��return" + -1);

                    return -1;
                }
                else
                {
                    Debug.Log(xName + "_" + yName + "_��return" + 1);
                    return 1;
                }
            }


            if (x.name.Length < y.name.Length)
            {
                    Debug.Log(xName + "_" + yName + "_��return" + -1);
                return -1;
            }
            else
            {
                    Debug.Log(xName + "_" + yName + "_��return" + 1);
                return 1;
            }

            SortingRating ChaScore(char c)
            {
                switch (c)
                {
                    case ':':
                        return SortingRating.Symbol;
                    case '/':
                        return SortingRating.Number;
                    default:
                        return SortingRating.Character;
                }
            }
        });

        sortFile.Sort((x, y) =>
        {
            string xName = "";
            foreach (var item in x.name)
            {
                xName += item;
            }
            string yName = "";
            foreach (var item in y.name)
            {
                yName += item;
            }
            //Debug.Log("<color=yellow>" + +"</color>");

            SortingRating xScore = 0;
            SortingRating yScore = 0;
            int numsIndex = -1;
            int symbolsIndex = -1;
            int nameLength = Math.Min(x.name.Length, y.name.Length);
            for (int i = 0; i < nameLength; i++)
            {
                xScore = ChaScore(x.name[i]);
                yScore = ChaScore(y.name[i]);

                if (xScore == yScore)
                {
                    switch (xScore)
                    {
                        case SortingRating.Number:
                            numsIndex++;
                            if (x.num[numsIndex] == y.num[numsIndex]) continue;
                            Debug.Log(yName + "_" + xName + "_��return" + x.num[numsIndex].CompareTo(y.num[numsIndex]));
                            return x.num[numsIndex].CompareTo(y.num[numsIndex]);

                        case SortingRating.Symbol:
                            symbolsIndex++;
                            if (x.symbol[symbolsIndex].Equals(y.symbol[symbolsIndex])) continue;
                            Debug.Log(xName + "_" + yName + "_��return" + x.symbol[symbolsIndex].CompareTo(y.symbol[symbolsIndex]));
                            return x.symbol[symbolsIndex].CompareTo(y.symbol[symbolsIndex]);
                    }
                }
                else if ((int)xScore > (int)yScore)
                {
                    Debug.Log(xName + "_" + yName + "_��return" + 1);

                    return 1;
                }
                else
                {
                    Debug.Log(xName + "_" + yName + "_��return" + -1);
                    return -1;
                }
            }


            if (x.name.Length > y.name.Length)
            {
                Debug.Log(xName + "_" + yName + "_��return" + -1);
                return -1;
            }
            else
            {
                Debug.Log(xName + "_" + yName + "_��return" + 1);
                return 1;
            }

            SortingRating ChaScore(char c)
            {
                switch (c)
                {
                    case ':':
                        return SortingRating.Symbol;
                    case '/':
                        return SortingRating.Number;
                    default:
                        return SortingRating.Character;
                }
            }
        });
        //*/

        FileFoldersGrantData[] returnGrantDatas = new FileFoldersGrantData[sortGrantDatas.Length];
        int returnGrantDatasIndex = 0;
        for (int i = 0; i < sortFolder.Count; i++)
        {
            returnGrantDatas[returnGrantDatasIndex] = sortGrantDatas[sortFolder[i].index];

            returnGrantDatasIndex++;
        }
        for (int i = 0; i < sortFile.Count; i++)
        {
            returnGrantDatas[returnGrantDatasIndex] = sortGrantDatas[sortFile[i].index];

            returnGrantDatasIndex++;
        }

        if (!ascendingOrder) returnGrantDatas.Reverse();

        return returnGrantDatas;

    }
}
