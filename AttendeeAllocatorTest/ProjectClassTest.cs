using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using AttendeeAllocator;

namespace AttendeeAllocatorTest
{
    [TestFixture]
    public class ProjectClassTest
    {
        [TestCase("..//..//testlist1.csv")]
        public void CsvReadTest(string filePath)
        {
            Project proj = new Project();
            bool result;
            CsvData data = proj.AttendeeData;
            result = proj.OpenCsvFile( data, filePath);
            //リストには５件の参加者登録
            Assert.AreEqual(result, true);
            Assert.AreEqual(proj.AttendeeNum, 6); // 見出し行を入れて６行

            //最初の参加者の登録情報確認
            List<string[]> test1 = proj.AttendeeData.Rows;

            //見出し行
            string[] row = test1[0];
            Assert.AreEqual(row[0], "受付番号");
            Assert.AreEqual(row[1], "氏名(姓)");
            Assert.AreEqual(row[2], "氏名(名)");
            Assert.AreEqual(row[3], "ふりがな(姓)");
            Assert.AreEqual(row[4], "ふりがな(名)");
            Assert.AreEqual(row[5], "会社名・学校名");
            Assert.AreEqual(row[6], "所属");
            Assert.AreEqual(row[7], "学年");
            Assert.AreEqual(row[8], "郵便番号");
            Assert.AreEqual(row[9], "都道府県");
            Assert.AreEqual(row[10], "住所1");
            Assert.AreEqual(row[11], "住所2");
            Assert.AreEqual(row[12], "電話番号");
            Assert.AreEqual(row[13], "FAX");
            Assert.AreEqual(row[14], "電子メール");
            Assert.AreEqual(row[15], "参加区分");
            Assert.AreEqual(row[16], "会員区分");
            Assert.AreEqual(row[17], "会員番号");
            Assert.AreEqual(row[18], "性別");
            Assert.AreEqual(row[19], "喫煙");
            Assert.AreEqual(row[20], "参加回数");
            Assert.AreEqual(row[21], "テーマ");
            Assert.AreEqual(row[22], "ML登録");
            Assert.AreEqual(row[23], "請求書");
            Assert.AreEqual(row[24], "請求書宛名");
            Assert.AreEqual(row[25], "館内ツアー");
            Assert.AreEqual(row[26], "備考");
            Assert.AreEqual(row[27], "申込日");
            Assert.AreEqual(row[28], "PP");

            //一人目
            row = test1[1];
            Assert.AreEqual(row[0],"1");
            Assert.AreEqual(row[1],"AAA1");
            Assert.AreEqual(row[2], "BBB1");
            Assert.AreEqual(row[3], "みょうじ１");
            Assert.AreEqual(row[4], "なまえ２");
            Assert.AreEqual(row[5], "CCC1");
            Assert.AreEqual(row[6], "DDD1");
            Assert.AreEqual(row[7], "EEE1");
            Assert.AreEqual(row[8], "000-0001");
            Assert.AreEqual(row[9], "○○県");
            Assert.AreEqual(row[10], "○○市");
            Assert.AreEqual(row[11], "ああ１");
            Assert.AreEqual(row[12], "000-0000-0001");
            Assert.AreEqual(row[13], "111-1111-0001");
            Assert.AreEqual(row[14], "a1@bc.de");
            Assert.AreEqual(row[15], "会員");
            Assert.AreEqual(row[16], "SESSAME");
            Assert.AreEqual(row[17], "");
            Assert.AreEqual(row[18], "男");
            Assert.AreEqual(row[19], "有");
            Assert.AreEqual(row[20], "はじめて");
            Assert.AreEqual(row[21], "その他");
            Assert.AreEqual(row[22], "登録済み");
            Assert.AreEqual(row[23], "必要");
            Assert.AreEqual(row[24], "あああ");
            Assert.AreEqual(row[25], "希望しない");
            Assert.AreEqual(row[26], "めも１");
            Assert.AreEqual(row[27], "20140101");
            Assert.AreEqual(row[28], "---");

            //二人目
            row = test1[2];
            Assert.AreEqual(row[0], "2");
            Assert.AreEqual(row[1], "AAA2");
            Assert.AreEqual(row[2], "BBB2");
            //Assert.AreEqual(row[3], "みょうじ１");
            //Assert.AreEqual(row[4], "なまえ２");
            //Assert.AreEqual(row[5], "CCC1");
            //Assert.AreEqual(row[6], "DDD1");
            //Assert.AreEqual(row[7], "EEE1");
            //Assert.AreEqual(row[8], "000-0001");
            //Assert.AreEqual(row[9], "○○県");
            //Assert.AreEqual(row[10], "○○市");
            //Assert.AreEqual(row[11], "ああ１");
            //Assert.AreEqual(row[12], "000-0000-0001");
            //Assert.AreEqual(row[13], "111-1111-0001");
            //Assert.AreEqual(row[14], "a1@bc.de");
            //Assert.AreEqual(row[15], "会員");
            //Assert.AreEqual(row[16], "SESSAME");
            //Assert.AreEqual(row[17], "");
            Assert.AreEqual(row[18], "女");
            Assert.AreEqual(row[19], "無");
            Assert.AreEqual(row[20], "2回目");
            //Assert.AreEqual(row[21], "その他");
            //Assert.AreEqual(row[22], "登録済み");
            //Assert.AreEqual(row[23], "必要");
            //Assert.AreEqual(row[24], "あああ");
            //Assert.AreEqual(row[25], "希望しない");
            Assert.AreEqual(row[26], "めも２");
            Assert.AreEqual(row[27], "20140102");
            Assert.AreEqual(row[28], "提出済");


            //３人目
            row = test1[3];
            Assert.AreEqual(row[0], "3");
            Assert.AreEqual(row[1], "AAA3");
            Assert.AreEqual(row[2], "BBB3");
            //Assert.AreEqual(row[3], "みょうじ１");
            //Assert.AreEqual(row[4], "なまえ２");
            //Assert.AreEqual(row[5], "CCC1");
            //Assert.AreEqual(row[6], "DDD1");
            //Assert.AreEqual(row[7], "EEE1");
            //Assert.AreEqual(row[8], "000-0001");
            //Assert.AreEqual(row[9], "○○県");
            //Assert.AreEqual(row[10], "○○市");
            //Assert.AreEqual(row[11], "ああ１");
            //Assert.AreEqual(row[12], "000-0000-0001");
            //Assert.AreEqual(row[13], "111-1111-0001");
            //Assert.AreEqual(row[14], "a1@bc.de");
            //Assert.AreEqual(row[15], "会員");
            //Assert.AreEqual(row[16], "SESSAME");
            //Assert.AreEqual(row[17], "");
            Assert.AreEqual(row[18], "男");
            Assert.AreEqual(row[19], "無");
            Assert.AreEqual(row[20], "3回目");
            //Assert.AreEqual(row[21], "その他");
            //Assert.AreEqual(row[22], "登録済み");
            //Assert.AreEqual(row[23], "必要");
            //Assert.AreEqual(row[24], "あああ");
            //Assert.AreEqual(row[25], "希望しない");
            Assert.AreEqual(row[26], ""); //備考欄は空欄
            //Assert.AreEqual(row[27], "20140102");
            //Assert.AreEqual(row[28], "提出済");


            //四人目
            row = test1[4];
            Assert.AreEqual(row[0], "4");
            Assert.AreEqual(row[1], "AAA4");
            Assert.AreEqual(row[2], "BBB4");
            //Assert.AreEqual(row[3], "ふりがな(姓)");
            //Assert.AreEqual(row[4], "ふりがな(名)");
            //Assert.AreEqual(row[5], "会社名・学校名");
            //Assert.AreEqual(row[6], "所属");
            //Assert.AreEqual(row[7], "学年");
            //Assert.AreEqual(row[8], "郵便番号");
            //Assert.AreEqual(row[9], "都道府県");
            //Assert.AreEqual(row[10], "住所1");
            //Assert.AreEqual(row[11], "住所2");
            //Assert.AreEqual(row[12], "電話番号");
            //Assert.AreEqual(row[13], "FAX");
            //Assert.AreEqual(row[14], "電子メール");
            //Assert.AreEqual(row[15], "参加区分");
            //Assert.AreEqual(row[16], "会員区分");
            //Assert.AreEqual(row[17], "会員番号");
            Assert.AreEqual(row[18], "女");
            Assert.AreEqual(row[19], "有");
            Assert.AreEqual(row[20], "4～6回目");
            //Assert.AreEqual(row[21], "テーマ");
            //Assert.AreEqual(row[22], "ML登録");
            //Assert.AreEqual(row[23], "請求書");
            //Assert.AreEqual(row[24], "請求書宛名");
            //Assert.AreEqual(row[25], "館内ツアー");
            Assert.AreEqual(row[26], "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０");
            //Assert.AreEqual(row[27], "申込日");
            //Assert.AreEqual(row[28], "PP");


            //五人目
            row = test1[5];
            Assert.AreEqual(row[0], "5");
            Assert.AreEqual(row[1], "AAA5");
            Assert.AreEqual(row[2], ""); //空欄
            Assert.AreEqual(row[3], "みょうじ５");
            Assert.AreEqual(row[4], "");　//空欄
            //Assert.AreEqual(row[5], "会社名・学校名");
            //Assert.AreEqual(row[6], "所属");
            //Assert.AreEqual(row[7], "学年");
            //Assert.AreEqual(row[8], "郵便番号");
            //Assert.AreEqual(row[9], "都道府県");
            //Assert.AreEqual(row[10], "住所1");
            //Assert.AreEqual(row[11], "住所2");
            //Assert.AreEqual(row[12], "電話番号");
            //Assert.AreEqual(row[13], "FAX");
            //Assert.AreEqual(row[14], "電子メール");
            //Assert.AreEqual(row[15], "参加区分");
            //Assert.AreEqual(row[16], "会員区分");
            //Assert.AreEqual(row[17], "会員番号");
            Assert.AreEqual(row[18], "男");
            Assert.AreEqual(row[19], "無");
            Assert.AreEqual(row[20], "7～9回目");
            //Assert.AreEqual(row[21], "テーマ");
            //Assert.AreEqual(row[22], "ML登録");
            //Assert.AreEqual(row[23], "請求書");
            //Assert.AreEqual(row[24], "請求書宛名");
            //Assert.AreEqual(row[25], "館内ツアー");
            Assert.AreEqual(row[26], ""); //空欄
            //Assert.AreEqual(row[27], "申込日");
            //Assert.AreEqual(row[28], "PP");
        
        }
    }
}
