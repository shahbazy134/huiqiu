﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;

namespace ChineseCharDemo
{
    class Program
    {
        static void Main(string[] args)
        {
/*******************输出结果****************************************************
'微'的笔画数是13。
其他相同笔画的中文字符有:
亂亃亄亶亷傪傫催傭傮傯傰傱傳傴債傶傷傸傹傺傻傼傽傾傿僀僁僂僄僅僆僇僈僉僋僌働像僙
兡兾兿凗剷剸剹剺剻剼剽剾剿勠勡勢勣勤勦勧匓匯厀厁厪叠喍喿嗀嗁嗂嗃嗄嗅嗆嗇嗈嗉嗊嗋
嗌嗍嗎嗐嗑嗒嗓嗔嗕嗘嗙嗚嗛嗜嗝嗠嗡嗣嗤嗥嗦嗧嗨嗩嗪嗫嗬嗮嗯嗰嗱嗲嗳嗵嗶嗷嗸嗹嗼嘟
嘩圑園圓圔圕堽塉塋塌塍塎塏塐塑塒塓塕塖塗塘塙塚塛塜塝塞塡塢塣塤塥塧塨塪填塬塮塯塱
塳塻墎墓墷壼壾夢奦媰媱媲媳媴媵媷媸媹媺媻媼媽媾嫀嫁嫃嫄嫆嫇嫈嫉嫊嫋嫌嫍嫎嫐嫑嫒嫓
嫔嫟嫫嫯嬅嬋孴孶寖寗寘寙寚寛寜寝寞尟尠尲尴嵊嵞嵟嵠嵡嵢嵣嵤嵥嵦嵧嵨嵩嵪嵭嵮嵰嵱嵲
嵴嵵嶅幊幋幌幍幎幏幐幕幙幹廅廇廈廉廌廍廒廓廕弒弿彀彂彃彙彚彮徬徭微徯徰想惷愁愂愆
愈愍意愗愙愚愛感愩愪愫愭愮愯愰愱愴愵愶愷愹愼愽愾慀慃慄慆慈慉慊慍慎慏慑慔慠慥慩慪
戣戤戥揅揧揫揱搆搇搈搉搊搋搌損搎搏搐搑搒搕搖搗搘搙搚搛搝搞搟搠搡搢搣搤搦搧搨搩搪
搬搮搯搱搲搳搵搶搷搸搹携搼搾摀摁摃摄摅摆摇摈摉摊摋摓摙摛摸撶敫敭敮敯数斒斞斟新斱
旒旓晸暄暅暆暇暈暉暊暋暌暍暐暒暓暔暕暖暗暘暙暛會朡棩椯椱椲椳椴椵椶椷椸椹椺椻椼椽
椾椿楀楁楂楃楄楅楆楈楊楋楌楍楎楏楐楑楒楓楔楕楘楙楚楜楝楞楟楠楡楢楣楤楥楦楨楩楪楫
楬業楯楱楲楳楴楶楷楸楹楺楻楼楽榀概榃榄榅榆榇榈榉榌榘榙榳榵榾槆槌槎槐歀歁歂歃歅歆
歇歈歌歱歲殜殟殾殿毀毁毂毷毸毹毺毻毼氱氲湬溍溎溏源溑溒溓溔溕準溗溘溙溛溜溝溟溡溢
溣溤溥溦溧溨溩溪溫溬溭溮溯溰溱溳溴溵溶溷溸溹溺溻溼溽溾溿滀滂滃滄滅滆滇滈滉滊滌滍
滏滐滒滓滔滖滗滘滙滚滛滜滝滟滠满滢滣滤滥滦滧滨滩滪滭滶漓漠漣漨漭漷煁煂煃煄煅煆煇
煈煉煊煋煌煍煎煏煒煓煔煕煖煗煘煙煜煝煞煟煠煢煣煤煥煦照煨煩煪煫煬煭煯煰煲煳煴煵煶
煷煸煺熍熓牃牎牏牐牑牒犌犎犏犐犑献猷猺猻猼猽猿獁獂獅獆獈獉獊獏獒獓琧琾琿瑀瑁瑂瑃
瑄瑅瑆瑇瑈瑉瑊瑋瑌瑍瑎瑏瑐瑑瑒瑓瑔瑕瑖瑗瑙瑚瑜瑝瑞瑟瑥瑰瑳瑵瓡甂甃甄甆甝甞畵當畷
畸畹畺痬痭痮痯痰痱痲痳痴痵痶痷痸痹痺痻痼痽痿瘀瘁瘂瘃瘄瘅瘆瘏瘐瘑瘔皗皘皙皵盝盞盟
睒睓睔睕睖睗睘睙睚睛睜睝睞睟睠睡睢督睤睥睦睧睨睩睪睫睬睭睰睷睹瞄矠矮硸硹硺硻硼硽
硾硿碀碁碂碃碄碅碆碇碈碉碊碋碌碍碎碏碐碑碒碓碔碕碖碗碘碙碚碛碜碢碤碰禀禁禈禉禊禋
禌禎福禐禑禒禓禔禕禖禗禘禙稏稐稑稒稓稔稕稖稗稘稙稚稛稜稝稞稟稠稡稢稣稤稥窞窟窠窡
窢窣窤窥窦窧窩竨竩竪筞筟筠筡筢筣筤筦筧筨筩筪筫筭筮筯筰筱筲筴筶筷筸筹筺筻筼筽签筿
简節粮粯粰粱粲粳粴粵絸絹絺絻絼絿綀綂綃綄綅綆綇綈綉綊綋綌綍綏綐綑綒經綔綕綗綘継綤
缚缛缜缝缞缟缠缡缢缣缤罧罨罩罪罫罬罭置署羣群羥羦羧羨義翜翝耡耢聕聖聗聘肄肅肆腛腜
腝腞腟腠腢腣腤腥腦腧腨腩腪腫腬腭腮腯腰腲腳腵腶腷腸腹腺腻腼腽腾腿膄膇舅與艀艁艂艃
艄艅艆艈艉蒑蒒蒓蒔蒕蒖蒗蒘蒙蒚蒛蒜蒝蒞蒟蒠蒡蒣蒤蒥蒦蒧蒨蒩蒪蒬蒭蒮蒯蒰蒱蒲蒳蒴蒵
蒶蒷蒸蒹蒺蒻蒼蒽蒿蓀蓁蓂蓄蓅蓆蓉蓊蓋蓌蓍蓎蓏蓐蓑蓒蓓蓔蓕蓖蓗蓘蓛蓝蓞蓟蓠蓡蓢蓣蓤
蓥蓦蓧蓨蓩蓪蓫蓬蓮蓽蔀蔭蔯蔱虜虞號虡蛖蛵蛶蛷蛸蛹蛺蛻蛼蛽蛾蛿蜁蜂蜃蜄蜅蜆蜇蜈蜉蜊
蜋蜌蜍蜎蜏蜐蜔蜕蜖蜗蜣蜹蝆蝍衘衙裊裏裔裘裚裛裝裟裠裧裨裩裪裬裭裮裯裰裱裲裶裷裸裺
裼裾裿褀褂褃褚覛覜觎觜觟觠觡觢解觤觥触觧訾訿詡詢詣詤詥試詧詨詩詪詫詬詭詮詯詰話該
詳詴詵詶詷詸詹詺詻詼詽詾詿誀誁誂誃誄誅誆誇誈誉誊誔誕誠谨谩谪谫谬谼豊豋豢豣豤豥豦
貄貅貆貇貈貉貊貲賂賃賄賅賆資賈賉賊賋賌賍賎赖赗赨赩赪趌趍趎趏趐趑趒趓趔跐跟跠跡跢
跣跤跥跦跧跨跩跪跫跬跭跮路跰跱跲跳跴跶跷跸跹跺跻躱躲躳軭軾軿輀輁輂較輄輅輆輇輈載
輊輋輌辏辐辑辒输辔辞辟辠農遘遙遚遛遜遝遞遟遠遡遢遣遤遥遨遳郌郒鄘鄙鄛鄜鄝鄞鄟鄠鄡
鄢鄣鄤鄥酧酨酩酪酫酬酭酮酯酰酱鈮鈯鈰鈱鈲鈳鈴鈵鈶鈷鈸鈹鈺鈻鈼鈽鈾鈿鉀鉁鉂鉃鉄鉆鉇
鉈鉉鉊鉋鉌鉍鉎鉏鉐鉑鉒鉓鉔鉕鉖鉗鉘鉙鉚鉛鉜鉝鉞鉟鉠鉡鉢鉣鉤鉥鉦鉧鉨鉩鉪鉫鉬鉭鉮鉰
鉱鉲鉳鉴銏锖锗锘错锚锛锜锝锞锟锠锡锢锣锤锥锦锧锨锩锪锫锬锭键锯锰锱锳镻閘閙閚閛閜
閝閞閟閠阖阗阘阙隚際障隝隟隠隡雉雊雍雎雏雴雵零雷雸雹雺電雼雽雾靕靖靲靳靴靵靶靷靹
韪韫韴韵頉頊頋頌頍頎頏預頑頒頓颐频颒颓颔颕颖颫颬飔飕飬飮飱飳飴飵飶飷飹飻飼飽飾飿
餀馌馍馎馏馐馚馯馰馱馲馳馴馵馺骜骝骞骟骯骰骱髡髢鬾鬿魀魁魂魛魜魝鲄鲅鲆鲇鲈鲉鲊鲋
鲌鲍鲎鲏鲐鳧鳨鳩鳪鳫鳭鳮鳯鹉鹊鹋鹌鹍鹎鹏鹐鹑鹒鹓鹔麀麁麂黽鼌鼓鼔鼠龃龄龅龆
1
'微'的拼音是"WEI1"。
共有37中文字符的拼音是"WEI1"。
'微'和'薇'同音。
******************************************************************************8*/
            ChineseChar chineseChar = null;
            char chChar = '微';
            char chCharToCompare = '薇';
            short stStrokeNumber = 0;//笔画数

            if (ChineseChar.IsValidChar(chChar))//判断'微'是否为有效中文字符
            {
                chineseChar = new ChineseChar(chChar);//
            }

            stStrokeNumber = chineseChar.StrokeNumber;//获得中文字符的笔画数
            Console.WriteLine("'" + chChar + "'的笔画数是{0}。", stStrokeNumber);


            Console.WriteLine("其他相同笔画的中文字符有:");
            Console.WriteLine(ChineseChar.GetChars(stStrokeNumber));//获得其他相同笔画的中文字符

            //Console.WriteLine(chineseChar.PinyinCount);//获取字符的拼音个数。

            foreach (string strPinyin in chineseChar.Pinyins)
            {
                if (strPinyin != null)
                {
                    Console.WriteLine("'" + chChar + "'的拼音是\"{0}\"。", strPinyin);//
                    if (ChineseChar.IsValidPinyin(strPinyin))
                    {
                        Console.WriteLine("共有{0}中文字符的拼音是\"" + strPinyin + "\"。", ChineseChar.GetHomophoneCount(strPinyin));
                    }
                }
            }


            //判断二中文字符是否同音,以下判断也可以通过 chineseChar.IsHomophone('微') 来判断
            if (ChineseChar.IsHomophone(chChar, chCharToCompare))
            {
                Console.WriteLine("'" + chChar + "'和'" + chCharToCompare + "'同音。");
            }
            else
            {
                Console.WriteLine("'" + chChar + "'和'" + chCharToCompare + "'不同音。");
            }

            Console.ReadLine();
        }
    }
}