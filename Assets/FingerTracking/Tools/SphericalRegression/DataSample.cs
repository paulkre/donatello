﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SphericalRegression {

    public class DataSample {

        public Vector3[] data = new Vector3[285] {
        new Vector3(-0.2093908f, 0.6526684f, 0.1397686f),
        new Vector3(-0.2158945f, 0.5897326f, 0.1554359f),
        new Vector3(-0.2195514f, 0.5280364f, 0.168068f),
        new Vector3(-0.2207081f, 0.4843848f, 0.1726369f),
        new Vector3(-0.2205521f, 0.4593526f, 0.1722325f),
        new Vector3(-0.2276341f, 0.4319708f, 0.1751985f),
        new Vector3(-0.2341529f, 0.3969653f, 0.1794828f),
        new Vector3(-0.2316881f, 0.3773194f, 0.1759452f),
        new Vector3(-0.2329018f, 0.3549024f, 0.1734472f),
        new Vector3(-0.2326266f, 0.3397525f, 0.1741634f),
        new Vector3(-0.2322887f, 0.3310384f, 0.1752673f),
        new Vector3(-0.2318234f, 0.3272085f, 0.1752456f),
        new Vector3(-0.2311401f, 0.3260301f, 0.1733121f),
        new Vector3(-0.229478f, 0.323296f, 0.1750809f),
        new Vector3(-0.2274682f, 0.323862f, 0.1746546f),
        new Vector3(-0.227031f, 0.3231075f, 0.175015f),
        new Vector3(-0.2243802f, 0.3242454f, 0.1727632f),
        new Vector3(-0.2247186f, 0.3241748f, 0.1714591f),
        new Vector3(-0.2226075f, 0.3248816f, 0.1730026f),
        new Vector3(-0.2229377f, 0.3234582f, 0.1715789f),
        new Vector3(-0.2211153f, 0.3303101f, 0.1726952f),
        new Vector3(-0.2150126f, 0.3581629f, 0.1726626f),
        new Vector3(-0.2069036f, 0.3938119f, 0.1767847f),
        new Vector3(-0.2006795f, 0.4461354f, 0.175866f),
        new Vector3(-0.1993539f, 0.5073568f, 0.1695929f),
        new Vector3(-0.2045718f, 0.5845413f, 0.1588334f),
        new Vector3(-0.2291293f, 0.6939886f, 0.1273068f),
        new Vector3(-0.2794296f, 0.8320224f, 0.06582912f),
        new Vector3(-0.3313208f, 0.915895f, -0.001941845f),
        new Vector3(-0.3367227f, 0.9477984f, -0.02090655f),
        new Vector3(-0.3268414f, 0.9843475f, -0.05934425f),
        new Vector3(-0.3206026f, 1.006722f, -0.080081f),
        new Vector3(-0.3161155f, 1.023425f, -0.09863988f),
        new Vector3(-0.309176f, 1.03608f, -0.1121043f),
        new Vector3(-0.3041414f, 1.044446f, -0.1195713f),
        new Vector3(-0.3031462f, 1.049246f, -0.1273851f),
        new Vector3(-0.3022333f, 1.055026f, -0.1336367f),
        new Vector3(-0.3046596f, 1.057553f, -0.1348865f),
        new Vector3(-0.3073719f, 1.058113f, -0.1367335f),
        new Vector3(-0.3054127f, 1.064456f, -0.1439147f),
        new Vector3(-0.311245f, 1.061724f, -0.1395686f),
        new Vector3(-0.3163275f, 1.059326f, -0.1401503f),
        new Vector3(-0.3241449f, 1.057449f, -0.1413137f),
        new Vector3(-0.3276131f, 1.052824f, -0.1398336f),
        new Vector3(-0.3343305f, 1.050663f, -0.1354631f),
        new Vector3(-0.3354898f, 1.050361f, -0.1277085f),
        new Vector3(-0.3369986f, 1.048396f, -0.1243474f),
        new Vector3(-0.337082f, 1.046484f, -0.1233082f),
        new Vector3(-0.3349052f, 1.046542f, -0.1205276f),
        new Vector3(-0.3370137f, 1.043663f, -0.1154102f),
        new Vector3(-0.3365035f, 1.042363f, -0.1155487f),
        new Vector3(-0.3314836f, 1.044266f, -0.1129306f),
        new Vector3(-0.3355436f, 1.039821f, -0.1133611f),
        new Vector3(-0.3386356f, 1.03126f, -0.1007087f),
        new Vector3(-0.3440596f, 1.01431f, -0.08442253f),
        new Vector3(-0.3519186f, 0.9833267f, -0.04714906f),
        new Vector3(-0.3551316f, 0.9418111f, -0.01185156f),
        new Vector3(-0.3461208f, 0.8952319f, 0.020229f),
        new Vector3(-0.3330804f, 0.8289009f, 0.05979348f),
        new Vector3(-0.3091582f, 0.7565054f, 0.09762887f),
        new Vector3(-0.2892277f, 0.6662018f, 0.128245f),
        new Vector3(-0.2665764f, 0.5690957f, 0.1538923f),
        new Vector3(-0.2587256f, 0.4763456f, 0.1657297f),
        new Vector3(-0.2607526f, 0.4090916f, 0.1707032f),
        new Vector3(-0.2682232f, 0.3683716f, 0.1720335f),
        new Vector3(-0.2628001f, 0.3523871f, 0.1707522f),
        new Vector3(-0.2620597f, 0.3345599f, 0.1698197f),
        new Vector3(-0.2663478f, 0.3001564f, 0.1663529f),
        new Vector3(-0.268845f, 0.2701062f, 0.1601627f),
        new Vector3(-0.2700068f, 0.244521f, 0.153457f),
        new Vector3(-0.2698994f, 0.2258139f, 0.1509239f),
        new Vector3(-0.2686638f, 0.2183262f, 0.1479559f),
        new Vector3(-0.2669981f, 0.2139943f, 0.1484169f),
        new Vector3(-0.2643519f, 0.213666f, 0.1479469f),
        new Vector3(-0.2634834f, 0.2175279f, 0.1490538f),
        new Vector3(-0.2620114f, 0.2218957f, 0.1504293f),
        new Vector3(-0.260914f, 0.2261905f, 0.1514851f),
        new Vector3(-0.2586738f, 0.2301032f, 0.152561f),
        new Vector3(-0.2586792f, 0.2353049f, 0.1531393f),
        new Vector3(-0.2587975f, 0.2402598f, 0.1541623f),
        new Vector3(-0.2576195f, 0.2462123f, 0.1560324f),
        new Vector3(-0.2561619f, 0.2572904f, 0.158382f),
        new Vector3(-0.2531287f, 0.2753393f, 0.1635716f),
        new Vector3(-0.2484518f, 0.3034799f, 0.1670519f),
        new Vector3(-0.2404471f, 0.3540344f, 0.1713353f),
        new Vector3(-0.2259753f, 0.4730873f, 0.1693569f),
        new Vector3(-0.2458665f, 0.6546979f, 0.1417956f),
        new Vector3(-0.3169466f, 0.8175138f, 0.07215537f),
        new Vector3(-0.369276f, 0.9049597f, 0.003955886f),
        new Vector3(-0.3757594f, 0.9349695f, -0.0168734f),
        new Vector3(-0.3768323f, 0.9629916f, -0.0453081f),
        new Vector3(-0.3751597f, 0.9842663f, -0.06889284f),
        new Vector3(-0.3675741f, 0.9997895f, -0.08375257f),
        new Vector3(-0.3585476f, 1.0114f, -0.1008115f),
        new Vector3(-0.3554755f, 1.016249f, -0.1037577f),
        new Vector3(-0.3512338f, 1.022415f, -0.102864f),
        new Vector3(-0.3473823f, 1.028844f, -0.1070789f),
        new Vector3(-0.3506589f, 1.026929f, -0.1076614f),
        new Vector3(-0.3514942f, 1.027008f, -0.1102795f),
        new Vector3(-0.3458625f, 1.032777f, -0.11218f),
        new Vector3(-0.3458522f, 1.030789f, -0.1155926f),
        new Vector3(-0.3504521f, 1.024323f, -0.1117287f),
        new Vector3(-0.351616f, 1.021783f, -0.1093736f),
        new Vector3(-0.3534657f, 1.016787f, -0.1047453f),
        new Vector3(-0.3566597f, 1.012922f, -0.09329308f),
        new Vector3(-0.3577423f, 1.009655f, -0.09039428f),
        new Vector3(-0.3585967f, 1.007002f, -0.08809969f),
        new Vector3(-0.3589759f, 1.003864f, -0.08469783f),
        new Vector3(-0.3591487f, 1.00184f, -0.08773923f),
        new Vector3(-0.3601235f, 0.9988202f, -0.08632228f),
        new Vector3(-0.3621454f, 0.9948882f, -0.08104224f),
        new Vector3(-0.3635854f, 0.9910308f, -0.07913239f),
        new Vector3(-0.356763f, 0.9888952f, -0.07356767f),
        new Vector3(-0.3585018f, 0.9766546f, -0.05773954f),
        new Vector3(-0.3576627f, 0.949585f, -0.03098629f),
        new Vector3(-0.3571863f, 0.8941606f, 0.009291731f),
        new Vector3(-0.3386448f, 0.8244299f, 0.05893566f),
        new Vector3(-0.320819f, 0.7441404f, 0.09894785f),
        new Vector3(-0.3100899f, 0.6642472f, 0.1274623f),
        new Vector3(-0.303345f, 0.6062962f, 0.139614f),
        new Vector3(-0.3006638f, 0.5673113f, 0.1475424f),
        new Vector3(-0.2930224f, 0.5288898f, 0.155894f),
        new Vector3(-0.2879725f, 0.4913211f, 0.1595575f),
        new Vector3(-0.285104f, 0.4682941f, 0.1609789f),
        new Vector3(-0.2829842f, 0.4427077f, 0.1660611f),
        new Vector3(-0.2807779f, 0.4251367f, 0.1656944f),
        new Vector3(-0.2824181f, 0.4136678f, 0.1650896f),
        new Vector3(-0.2886713f, 0.3850195f, 0.1639864f),
        new Vector3(-0.2925106f, 0.3615862f, 0.1645342f),
        new Vector3(-0.2876697f, 0.3530136f, 0.1624942f),
        new Vector3(-0.2872184f, 0.3362758f, 0.1617312f),
        new Vector3(-0.2917798f, 0.323332f, 0.1605976f),
        new Vector3(-0.2911758f, 0.3176156f, 0.1609684f),
        new Vector3(-0.2898586f, 0.3172371f, 0.1639566f),
        new Vector3(-0.2822162f, 0.3286076f, 0.1592873f),
        new Vector3(-0.2828107f, 0.3280702f, 0.1614675f),
        new Vector3(-0.2774573f, 0.3337449f, 0.1603432f),
        new Vector3(-0.2748743f, 0.3377982f, 0.1591201f),
        new Vector3(-0.2721735f, 0.3449231f, 0.1609878f),
        new Vector3(-0.2716438f, 0.3441089f, 0.161531f),
        new Vector3(-0.2719021f, 0.3456451f, 0.1619321f),
        new Vector3(-0.2716358f, 0.3548369f, 0.160411f),
        new Vector3(-0.2736749f, 0.3600645f, 0.1602899f),
        new Vector3(-0.2725325f, 0.3867563f, 0.1611304f),
        new Vector3(-0.2772699f, 0.4278591f, 0.1577339f),
        new Vector3(-0.2915974f, 0.515265f, 0.1508361f),
        new Vector3(-0.3431547f, 0.654166f, 0.1198064f),
        new Vector3(-0.4193571f, 0.7948538f, 0.04905894f),
        new Vector3(-0.4653938f, 0.8592474f, -0.004298203f),
        new Vector3(-0.4634452f, 0.8951617f, -0.03218107f),
        new Vector3(-0.4559505f, 0.9260998f, -0.05706057f),
        new Vector3(-0.4440038f, 0.9494119f, -0.07194832f),
        new Vector3(-0.4289809f, 0.9682813f, -0.08782163f),
        new Vector3(-0.418458f, 0.9804258f, -0.1031392f),
        new Vector3(-0.4166941f, 0.9877756f, -0.1054415f),
        new Vector3(-0.4154336f, 0.9914365f, -0.1064845f),
        new Vector3(-0.4143206f, 0.9933054f, -0.1119574f),
        new Vector3(-0.4159662f, 0.9956678f, -0.1153093f),
        new Vector3(-0.4134872f, 0.9999125f, -0.1200266f),
        new Vector3(-0.4158801f, 1.001045f, -0.1189857f),
        new Vector3(-0.4146524f, 1.00403f, -0.1281586f),
        new Vector3(-0.4118754f, 1.008696f, -0.1285606f),
        new Vector3(-0.4133309f, 1.006286f, -0.1272281f),
        new Vector3(-0.4170655f, 0.9999691f, -0.1251526f),
        new Vector3(-0.4159619f, 0.9981302f, -0.1231306f),
        new Vector3(-0.4139343f, 0.9970257f, -0.1185786f),
        new Vector3(-0.4171375f, 0.9912124f, -0.1105125f),
        new Vector3(-0.4200116f, 0.9864899f, -0.1062611f),
        new Vector3(-0.4221788f, 0.980028f, -0.0982292f),
        new Vector3(-0.4238452f, 0.9702355f, -0.09118039f),
        new Vector3(-0.4256824f, 0.9576721f, -0.08194655f),
        new Vector3(-0.4267507f, 0.9378926f, -0.06173205f),
        new Vector3(-0.4273983f, 0.9087149f, -0.03457747f),
        new Vector3(-0.4226972f, 0.8513672f, 0.00494644f),
        new Vector3(-0.4105458f, 0.7781063f, 0.05135302f),
        new Vector3(-0.3910547f, 0.6812452f, 0.09761255f),
        new Vector3(-0.3703132f, 0.5692805f, 0.1307202f),
        new Vector3(-0.3522949f, 0.4556853f, 0.1525445f),
        new Vector3(-0.3382938f, 0.3627305f, 0.157906f),
        new Vector3(-0.3343555f, 0.2932724f, 0.1546035f),
        new Vector3(-0.3276178f, 0.2698681f, 0.1520065f),
        new Vector3(-0.3230423f, 0.2617664f, 0.150508f),
        new Vector3(-0.3177968f, 0.2525983f, 0.1506398f),
        new Vector3(-0.3139049f, 0.2433872f, 0.1501745f),
        new Vector3(-0.3104478f, 0.2370517f, 0.1492696f),
        new Vector3(-0.3082191f, 0.2368493f, 0.1492889f),
        new Vector3(-0.3052118f, 0.2420966f, 0.1496197f),
        new Vector3(-0.3036392f, 0.2570865f, 0.1531895f),
        new Vector3(-0.3017589f, 0.2719407f, 0.1556627f),
        new Vector3(-0.3000838f, 0.2787774f, 0.1568667f),
        new Vector3(-0.2979248f, 0.282608f, 0.1570862f),
        new Vector3(-0.3015718f, 0.2774076f, 0.1574247f),
        new Vector3(-0.2961143f, 0.2831417f, 0.157723f),
        new Vector3(-0.2957517f, 0.2836598f, 0.1564854f),
        new Vector3(-0.2955557f, 0.2809879f, 0.1553422f),
        new Vector3(-0.2950532f, 0.2798776f, 0.1553114f),
        new Vector3(-0.2946367f, 0.2796226f, 0.1552307f),
        new Vector3(-0.2992667f, 0.2765647f, 0.1552616f),
        new Vector3(-0.2945096f, 0.2844771f, 0.1560326f),
        new Vector3(-0.2945107f, 0.3089732f, 0.1580282f),
        new Vector3(-0.2975714f, 0.3658384f, 0.1618632f),
        new Vector3(-0.3163699f, 0.4546416f, 0.1604201f),
        new Vector3(-0.3637128f, 0.5797685f, 0.1386372f),
        new Vector3(-0.4512809f, 0.7425548f, 0.06869014f),
        new Vector3(-0.502961f, 0.8436376f, -0.008464307f),
        new Vector3(-0.4986727f, 0.8833605f, -0.04158851f),
        new Vector3(-0.4836715f, 0.9215189f, -0.06444577f),
        new Vector3(-0.4744652f, 0.9479358f, -0.09227738f),
        new Vector3(-0.4668993f, 0.965367f, -0.102039f),
        new Vector3(-0.4644805f, 0.9762131f, -0.1143418f),
        new Vector3(-0.4603421f, 0.9841728f, -0.1182038f),
        new Vector3(-0.4635273f, 0.9830548f, -0.1203055f),
        new Vector3(-0.4652303f, 0.9840096f, -0.1211574f),
        new Vector3(-0.4659337f, 0.9841502f, -0.1214625f),
        new Vector3(-0.4666548f, 0.9838284f, -0.1226277f),
        new Vector3(-0.4634649f, 0.9867821f, -0.1249355f),
        new Vector3(-0.4620112f, 0.9878137f, -0.1246302f),
        new Vector3(-0.4577701f, 0.9910584f, -0.1261227f),
        new Vector3(-0.4599231f, 0.9870307f, -0.1248418f),
        new Vector3(-0.4622148f, 0.9854293f, -0.1220283f),
        new Vector3(-0.4637344f, 0.9835817f, -0.122844f),
        new Vector3(-0.4650327f, 0.9815791f, -0.1219856f),
        new Vector3(-0.4691424f, 0.9769466f, -0.1183788f),
        new Vector3(-0.4693167f, 0.9760959f, -0.1176315f),
        new Vector3(-0.4687618f, 0.9748633f, -0.1141534f),
        new Vector3(-0.4720046f, 0.9673873f, -0.1106142f),
        new Vector3(-0.4732441f, 0.9633092f, -0.105435f),
        new Vector3(-0.4737787f, 0.9614947f, -0.1033517f),
        new Vector3(-0.4733629f, 0.9599613f, -0.09791046f),
        new Vector3(-0.4728061f, 0.956229f, -0.09630311f),
        new Vector3(-0.4749344f, 0.9524904f, -0.09463046f),
        new Vector3(-0.4750716f, 0.9471207f, -0.08931704f),
        new Vector3(-0.4752604f, 0.9398603f, -0.08242588f),
        new Vector3(-0.4790114f, 0.9223792f, -0.06454263f),
        new Vector3(-0.4745537f, 0.8940837f, -0.04208755f),
        new Vector3(-0.4678217f, 0.8495737f, -0.007362217f),
        new Vector3(-0.448562f, 0.7888406f, 0.03765859f),
        new Vector3(-0.4207555f, 0.6995165f, 0.08246318f),
        new Vector3(-0.3928768f, 0.6007555f, 0.1213324f),
        new Vector3(-0.3723499f, 0.5132157f, 0.1416369f),
        new Vector3(-0.3621961f, 0.4345637f, 0.1531566f),
        new Vector3(-0.3487864f, 0.3671536f, 0.1553467f),
        new Vector3(-0.3342766f, 0.2945227f, 0.15101f),
        new Vector3(-0.3252571f, 0.2033152f, 0.1395196f),
        new Vector3(-0.3179049f, 0.1396399f, 0.1226644f),
        new Vector3(-0.3218593f, 0.117427f, 0.111955f),
        new Vector3(-0.3219202f, 0.1147314f, 0.1107184f),
        new Vector3(-0.3197704f, 0.1145199f, 0.1109207f),
        new Vector3(-0.3191915f, 0.115206f, 0.1101882f),
        new Vector3(-0.3171226f, 0.1164071f, 0.1110462f),
        new Vector3(-0.317338f, 0.116743f, 0.1113026f),
        new Vector3(-0.318591f, 0.1164179f, 0.1126702f),
        new Vector3(-0.3184054f, 0.1163841f, 0.1130671f),
        new Vector3(-0.3172304f, 0.1177214f, 0.1128256f),
        new Vector3(-0.3173605f, 0.1190911f, 0.113074f),
        new Vector3(-0.31628f, 0.1193821f, 0.1143119f),
        new Vector3(-0.3160779f, 0.1189122f, 0.113839f),
        new Vector3(-0.3161637f, 0.1184495f, 0.1155488f),
        new Vector3(-0.316591f, 0.1195095f, 0.1168099f),
        new Vector3(-0.3170433f, 0.1185399f, 0.1190399f),
        new Vector3(-0.3181054f, 0.1202868f, 0.1198026f),
        new Vector3(-0.319963f, 0.1360513f, 0.1235637f),
        new Vector3(-0.3286435f, 0.1793213f, 0.1327012f),
        new Vector3(-0.3401375f, 0.2486984f, 0.148695f),
        new Vector3(-0.3599766f, 0.3275717f, 0.1534311f),
        new Vector3(-0.3676063f, 0.4044956f, 0.1544165f),
        new Vector3(-0.3737101f, 0.4478769f, 0.1485983f),
        new Vector3(-0.3798718f, 0.4914433f, 0.140859f),
        new Vector3(-0.3851671f, 0.5506327f, 0.1283844f),
        new Vector3(-0.3962514f, 0.6437246f, 0.1056503f),
        new Vector3(-0.4195656f, 0.7342589f, 0.06689921f),
        new Vector3(-0.4463271f, 0.794365f, 0.0329582f),
        new Vector3(-0.4628929f, 0.8539723f, -0.006325021f),
        new Vector3(-0.465767f, 0.8902356f, -0.03740191f),
        new Vector3(-0.461407f, 0.9245133f, -0.05815595f),
        new Vector3(-0.4596575f, 0.9491694f, -0.08151829f),
        new Vector3(-0.4578587f, 0.9674792f, -0.1006583f),
        new Vector3(-0.4607478f, 0.9770668f, -0.1124732f),
        new Vector3(-0.4600506f, 0.9850724f, -0.1243239f),
        new Vector3(-0.4540277f, 0.9954193f, -0.1385444f),
        new Vector3(-0.4556027f, 0.9997728f, -0.1433574f),
        new Vector3(-0.4565265f, 1.002361f, -0.1346662f),
        new Vector3(-0.4576541f, 1.00215f, -0.134572f),
        new Vector3(-0.4592661f, 0.998713f, -0.1417015f),
        new Vector3(-0.4595547f, 0.9982132f, -0.1413748f)
    };
    }
}
