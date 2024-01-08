using System.Net.Security;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;
using static System.Console;
using static Text_Game_.Program;

namespace Text_Game_
{
    // 확장 메서드는 최상위 정적 클래스에 위치해야함
    public static class ExtensionString
    {
        // string의 확장 메서드
        // 텍스트 색상을 변경하여 출력해줌
        // 콘솔에 입력할 텍스트, 색상, WriteLine or Write (bool 값으로 정의)
        public static void PrintWithColor(this string text, ConsoleColor color, bool isEnter)
        {
            ForegroundColor = color;
            if (isEnter) WriteLine(text); else Write(text);
            ResetColor();
        }
    }

    internal class Program
    {
        static Character player;
        static Item[] items;

        static int startMe;

        static void Main(string[] args)
        {
            GameDataSetting();
            PrintStartLogo();

            while (true)
            {
                if (startMe == 0) DisplayStartMenu();
                else if (startMe == 1) DisplayCharacterInfo();
                else if (startMe == 2) DisplayInventory();
                else if (startMe == 3) DisplayStore();
            }
        }

        static void PrintStartLogo()
        {
            WriteLine("      _________                        __                     "  );
            WriteLine("     /   _____/______ _____  _______ _/  |_ _____             "  );
            WriteLine("     \\_____  \\ \\____ \\\\__  \\ \\_  __ \\\\   __\\\\__  \\"  );
            WriteLine("     /        \\|  |_> >/ __ \\_|  | \\/ |  |   / __ \\_      "  );
            WriteLine("    /_______  /|   __/(____  /|__|    |__|  (____  /          "  );
            WriteLine("            \\/ |__|        \\/                    \\/        "  );
            WriteLine();
            WriteLine("________                     ____                          "     );
            WriteLine("\\______ \\   __ __   ____    / ___\\   ____   ____    ____   "  );
            WriteLine(" |    |  \\ |  |  \\ /    \\  / /_/  >_/ __ \\ /  _ \\  /    \\" );
            WriteLine(" |    `   \\|  |  /|   |  \\ \\___  / \\  ___/(  <_> )|   |  \\ ");
            WriteLine("/_______  /|____/ |___|  //_____/   \\___  >\\____/ |___|  / "   );
            WriteLine("        \\/             \\/               \\/             \\/  " );
            ("===========================================================").PrintWithColor(ConsoleColor.Yellow, true);
            ("                   Press Anykey To Start                   ").PrintWithColor(ConsoleColor.Yellow, true);
            ("===========================================================").PrintWithColor(ConsoleColor.Yellow, true);
            ReadKey();
        }

        // 게임 데이터 설정
        static void GameDataSetting()
        {
            player = new Character(10, "Sinsa", "전사", 10, 5, 100, 1500);
            items = new Item[10];
            AddItem(new Item("무쇠갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다.", 0, 0, 5, 1300));
            AddItem(new Item("낡은 검", "쉽게 볼 수 있는 낡은 검입니다.", 1, 2, 0, 600));
        }

        static void AddItem(Item item)
        {
            if (Item.ItemCnt == 10) return;
            items[Item.ItemCnt] = item;     // 0개 -> 0번 인덱스 / 1개 -> 1번 인덱스
            Item.ItemCnt++;
        }

        // 메인 화면 출력 메서드
        static void DisplayStartMenu()
        {
            Clear();
            WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
            WriteLine();
            ("1. ").PrintWithColor(ConsoleColor.Magenta, false); WriteLine("상태 보기");
            ("2. ").PrintWithColor(ConsoleColor.Magenta, false); WriteLine("인벤토리");
            ("3. ").PrintWithColor(ConsoleColor.Magenta, false); WriteLine("상점");
            WriteLine();
            startMe = GetPlayerSelect(1, 3);
        }

        // 캐릭터 상태 표시
        static void DisplayCharacterInfo()
        {
            Clear();
            ("상태 보기").PrintWithColor(ConsoleColor.Yellow, true);
            WriteLine("캐릭터의 정보가 표시됩니다.");
            WriteLine();
            Write("Lv. "); ($"{player.Level}").PrintWithColor(ConsoleColor.Magenta, true);
            WriteLine($"{player.Name} ( {player.Job} ) ");

            int bonusAtk = getSumBonusAtk();
            if (bonusAtk > 0)
            {
                Write("공격력 : "); ($"{player.Atk + bonusAtk}").PrintWithColor(ConsoleColor.Magenta, false);
                Write(" ("); ("+" + bonusAtk.ToString()).PrintWithColor(ConsoleColor.Magenta, false); WriteLine(")");
            }
            else
            {
                Write("공격력 : "); ($"{player.Atk}").PrintWithColor(ConsoleColor.Magenta, true);
            }

            int bonusDef = getSumBonusDef();
            if (bonusDef > 0)
            {
                Write("방어력 : "); ($"{player.Def + bonusDef}").PrintWithColor(ConsoleColor.Magenta, false);
                Write(" ("); ("+" + bonusDef.ToString()).PrintWithColor(ConsoleColor.Magenta, false); WriteLine(")");
            }
            else
            {
                Write("방어력 : "); ($"{player.Def}").PrintWithColor(ConsoleColor.Magenta, true);
            }

            Write("체 력 : "); ($"{player.Hp}").PrintWithColor(ConsoleColor.Magenta, true);
            Write("Gold : "); ($"{player.Gold}").PrintWithColor(ConsoleColor.Magenta, true);
            WriteLine();
            ("0. ").PrintWithColor(ConsoleColor.Magenta, false); WriteLine("나가기");
            WriteLine();
            startMe = GetPlayerSelect(0, 0);
        }

        static int getSumBonusAtk()
        {
            int sum = 0;
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                if (items[i].IsEquiped) sum += items[i].Atk;
            }
            return sum;
        }

        static int getSumBonusDef()
        {
            int sum = 0;
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                if (items[i].IsEquiped) sum += items[i].Def;
            }
            return sum;
        }

        // 인벤토리 표시
        static void DisplayInventory()
        {
            Clear();
            ("인벤토리 - 장착 관리").PrintWithColor(ConsoleColor.Yellow, true);
            WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            WriteLine();
            WriteLine("[아이템 목록]");
            WriteLine();
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                items[i].ItemDescription();
            }
            WriteLine();
            ("1. ").PrintWithColor(ConsoleColor.Magenta, false); WriteLine("장착 관리");
            ("0. ").PrintWithColor(ConsoleColor.Magenta, false); WriteLine("나가기");
            WriteLine();
            startMe = GetPlayerSelect(0, 1);
            switch (startMe)
            {
                case 0:
                    DisplayStartMenu();
                    break;
                case 1:
                    EquipItem();
                    break;
            }
        }

        private static void EquipItem()
        {
            Clear();
            ("인벤토리 - 장착 관리").PrintWithColor(ConsoleColor.Yellow, true);
            WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            WriteLine();
            WriteLine("[아이템 목록]");
            WriteLine();
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                items[i].ItemDescription(true, i + 1);
            }
            WriteLine();
            ("0. ").PrintWithColor(ConsoleColor.Magenta, false); WriteLine("나가기");
            WriteLine();
            startMe = GetPlayerSelect(0, Item.ItemCnt);
            switch (startMe)
            {
                case 0:
                    DisplayInventory();
                    break;
                default:
                    ToggleEquipStatus(startMe - 1);
                    EquipItem();
                    break;
            }
        }

        private static void ToggleEquipStatus(int idx)
        {
            items[idx].IsEquiped = !items[idx].IsEquiped;
        }

        static void DisplayStore()
        {
            Clear();
            ("상점 - 아이템 구매").PrintWithColor(ConsoleColor.Yellow, true);
            WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            WriteLine();
            WriteLine("[보유 골드]");
            (player.Gold.ToString()).PrintWithColor(ConsoleColor.Magenta, false);WriteLine(" G");
            WriteLine();
            WriteLine("[아이템 목록]");

            WriteLine();
            ("1. ").PrintWithColor(ConsoleColor.Magenta, false); WriteLine("아이템 구매");
            ("0. ").PrintWithColor(ConsoleColor.Magenta, false); WriteLine("나가기");
            WriteLine();
            startMe = GetPlayerSelect(0, 1);
            switch (startMe)
            {
                case 0:
                    DisplayStore();
                    break;
                case 1:
                    BuyItem();
                    break;
            }
        }

        private static void BuyItem()
        {
            throw new NotImplementedException();
        }

        // 메뉴 선택
        static int GetPlayerSelect(int start, int end)
        {
            WriteLine("원하시는 행동을 입력해주세요.");
            (">> ").PrintWithColor(ConsoleColor.Yellow, false);
            int select = 0;     // tryParse
            bool isNum = false; // While
            while (true)
            {
                isNum = int.TryParse(ReadLine(), out select);
                if (!isNum || (select < start || select > end))
                {
                    ("잘못된 입력입니다. 다시 고르세요").PrintWithColor(ConsoleColor.Red, true);
                }
                else break;
            }
            return select;
        }

        public class Character
        {
            public int Level { get; }
            public string Name { get; }
            public string Job { get; }      //Enum으로 대체 가능
            public int Atk { get; }
            public int Def { get; }
            public int Hp { get; }
            public int Gold { get; }

            public Character(int level, string name, string job, int atk, int def, int hp, int gold)
            {
                Level = level;
                Name = name;
                Job = job;
                Atk = atk;
                Def = def;
                Hp = hp;
                Gold = gold;
            }
        }

        public class Item
        {
            public string Name { get; }
            public string Description { get; }
            public int Type { get; }
            public int Atk { get; }
            public int Def { get; }
            public int Gold { get; }

            public bool IsEquiped { get; set; }     //장착 여부

            public static int ItemCnt = 0;

            public Item(string name, string description, int type, int atk, int def, int gold, bool isEquiped = false)
            {
                Name = name;
                Description = description;
                Type = type;
                Atk = atk;
                Def = def;
                Gold = gold;
                IsEquiped = isEquiped;
            }

            public void ItemDescription(bool withNumber = false, int idx = 0)
            {
                Write("- ");
                if (withNumber)
                {
                    ($"{idx}. ").PrintWithColor(ConsoleColor.Magenta, false);
                }
                if (IsEquiped)
                {
                    Write("["); ("E").PrintWithColor(ConsoleColor.Cyan, false); Write("]");
                    Write(PadRightForMixedText(Name, 9));
                }
                else Write(PadRightForMixedText(Name, 12));

                Write(" | ");

                if (Atk != 0) Write($"Atk {(Atk >= 0 ? "+" : "")}{Atk} ");
                if (Def != 0) Write($"Def {(Def >= 0 ? "+" : "")}{Def} ");

                Write(" | ");

                WriteLine(Description);
            }

            // 아이템 이름의 줄맞춤을 위한 메서드
            public static int GetPrintableLength(string str)
            {
                int length = 0;
                foreach (char c in str)
                {
                    if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                    {
                        length += 2;  // 한글인 경우 길이 2
                    }
                    else
                    {
                        length += 1;  // 한글이 아닌 경우 길이 1
                    }
                }
                return length;
            }

            public static string PadRightForMixedText(string str, int totalLength)
            {
                int currentLength = GetPrintableLength(str);  // 텍스트 실제 길이
                int padding = totalLength - currentLength;  // 원하는 길이 - 현재 길이
                return str.PadRight(str.Length + padding);  // 원하는 길이만큼 붙여주기
            }
        }
    }
}
