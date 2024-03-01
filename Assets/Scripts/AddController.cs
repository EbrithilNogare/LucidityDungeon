using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Assets.Scripts
{
    public class AddController : MonoBehaviour
    {
        void Start()
        {
            string[] links = new string[] {
                "https://rr5---sn-2gb7sne6.googlevideo.com/videoplayback?expire=1700796340&ei=VMNfZfG1HcCVv_IPpIi3gA8&ip=2a01%3A4f9%3A6b%3A252f%3A%3A2&id=o-APJdzdCrtyzaqkqPeoZWl3rlhKeOd67aeSyaOH0aufw0&itag=18&source=youtube&requiressl=yes&siu=1&vprv=1&mime=video%2Fmp4&cnr=14&ratebypass=yes&dur=57.422&lmt=1698391447955131&fexp=24007246&c=ANDROID_TESTSUITE&txp=4530434&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Csiu%2Cvprv%2Cmime%2Ccnr%2Cratebypass%2Cdur%2Clmt&sig=ANLwegAwRQIgC1fVWOeb9d11MeQdIzuK_uif3MEAl7kc5RL_si6Jq6YCIQCtdovfE2BytQaSmFLMlgOARG_o3ajmdy1VBljRWKv-fw%3D%3D&title=y2mate.is%20-%20Doru%C4%8Den%C3%AD+ZDARMA+s+AlzaPlus%2B+%7C+Jirka+Kr%C3%A1l-xeHly7zQ77A-360p-1700775583&rm=sn-5goly7l&req_id=9b7495dacc3ba3ee&redirect_counter=2&cm2rm=sn-8vq54voxj1-2gbs7e&cms_redirect=yes&cmsv=e&ipbypass=yes&mh=NQ&mip=2a02:8308:295:0:b841:6831:e1af:a697&mm=29&mn=sn-2gb7sne6&ms=rdu&mt=1700775409&mv=m&mvi=5&pl=40&lsparams=ipbypass,mh,mip,mm,mn,ms,mv,mvi,pl&lsig=AM8Gb2swRQIhAOvhjFyvlWHHGCRDWeVvs9hqC6Sec_uXU3A9SVdjIut_AiBHMUVr92sJcEQPGdX9PejpoelXLdUS2qcJp35bZkQmmw%3D%3D",
                "https://rr4---sn-2gb7sn7y.googlevideo.com/videoplayback?expire=1700796350&ei=XsNfZf60CtSm0u8PuM6NkAc&ip=2a01%3A4f9%3A3051%3A4442%3A%3A2&id=o-AI57TijLi6RZELhwLrsqZP2GLFfox9_GxtST-_H8TxRN&itag=18&source=youtube&requiressl=yes&siu=1&vprv=1&mime=video%2Fmp4&cnr=14&ratebypass=yes&dur=20.062&lmt=1679939344375174&fexp=24007246&c=ANDROID_TESTSUITE&txp=4438434&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Csiu%2Cvprv%2Cmime%2Ccnr%2Cratebypass%2Cdur%2Clmt&sig=ANLwegAwRAIgBgZpBHN-sAilmzkWPRmNLHgPR28aAl4tMh6jf_xU8M8CIEqM9TXb8pLv5VCef3rl8mRkbwrDJ_ayeLf-CGxrXzV_&title=y2mate.is%20-%20Alza+premium+v%C3%BDhody+na+Alza.sk-VpR4X4nH9EU-360p-1700774756&rm=sn-5golr7l&req_id=e6813be5b360a3ee&redirect_counter=2&cm2rm=sn-8vq54voxj1-2gbl7s&cms_redirect=yes&cmsv=e&ipbypass=yes&mh=m6&mip=2a02:8308:295:0:b841:6831:e1af:a697&mm=29&mn=sn-2gb7sn7y&ms=rdu&mt=1700774450&mv=m&mvi=4&pl=40&lsparams=ipbypass,mh,mip,mm,mn,ms,mv,mvi,pl&lsig=AM8Gb2swRQIgavOARmO1Xr8B_9hR8wuQxkBwIo90c7IQ-XaBoWiyVCACIQCTMkfpiq1mTSKwgq-TE2bNMMg__eQuk89qC9eeF4UvTw%3D%3D",
                "https://rr5---sn-2gb7sne6.googlevideo.com/videoplayback?expire=1700796347&ei=WsNfZb7iKY2Z0u8PhLOSwAU&ip=2a01%3A4f9%3A6b%3A4b18%3A%3A2&id=o-AAErurLukyGMt2I-v-vdIIkaFGddiKMO0ja5OPkCoezP&itag=18&source=youtube&requiressl=yes&siu=1&vprv=1&mime=video%2Fmp4&cnr=14&ratebypass=yes&dur=58.305&lmt=1696228920853947&fexp=24007246&c=ANDROID_TESTSUITE&txp=4530434&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Csiu%2Cvprv%2Cmime%2Ccnr%2Cratebypass%2Cdur%2Clmt&sig=ANLwegAwRQIhALo1Jc04tvr-lL6sSJWUcKOXaO551UqhrQWrj5zuKeauAiA6IjrWP-nS8gTcku6K1xUYwvgvgKnaMIq3bjn__gYidw%3D%3D&title=y2mate.is%20-%20Doru%C4%8Den%C3%AD+ZDARMA+s+AlzaPlus%2B+%7C+Nikol-qa-5pDibesk-360p-1700774750&rm=sn-5gos77z&req_id=bf8ab3141415a3ee&redirect_counter=2&cm2rm=sn-8vq54voxj1-2gbl7s&cms_redirect=yes&cmsv=e&ipbypass=yes&mh=rX&mip=2a02:8308:295:0:b841:6831:e1af:a697&mm=29&mn=sn-2gb7sne6&ms=rdu&mt=1700774450&mv=m&mvi=5&pl=40&lsparams=ipbypass,mh,mip,mm,mn,ms,mv,mvi,pl&lsig=AM8Gb2swRgIhANIG9-3gOUfndsYG72bx1w8HLizJds4m44HyKAx7EJymAiEAy_5JktY6az7cqsqGqMvYQwqz4lQ07gwlZ7pX91pwVKY%3D",
                "https://rr5---sn-2gb7sne6.googlevideo.com/videoplayback?expire=1700796340&ei=VMNfZfG1HcCVv_IPpIi3gA8&ip=2a01%3A4f9%3A6b%3A252f%3A%3A2&id=o-APJdzdCrtyzaqkqPeoZWl3rlhKeOd67aeSyaOH0aufw0&itag=18&source=youtube&requiressl=yes&siu=1&vprv=1&mime=video%2Fmp4&cnr=14&ratebypass=yes&dur=57.422&lmt=1698391447955131&fexp=24007246&c=ANDROID_TESTSUITE&txp=4530434&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Csiu%2Cvprv%2Cmime%2Ccnr%2Cratebypass%2Cdur%2Clmt&sig=ANLwegAwRQIgC1fVWOeb9d11MeQdIzuK_uif3MEAl7kc5RL_si6Jq6YCIQCtdovfE2BytQaSmFLMlgOARG_o3ajmdy1VBljRWKv-fw%3D%3D&title=y2mate.is%20-%20Doru%C4%8Den%C3%AD+ZDARMA+s+AlzaPlus%2B+%7C+Jirka+Kr%C3%A1l-xeHly7zQ77A-360p-1700774744&rm=sn-5goly7l&req_id=f12bc522eb08a3ee&redirect_counter=2&cm2rm=sn-8vq54voxj1-2gbs7e&cms_redirect=yes&cmsv=e&ipbypass=yes&mh=NQ&mip=2a02:8308:295:0:b841:6831:e1af:a697&mm=29&mn=sn-2gb7sne6&ms=rdu&mt=1700774450&mv=m&mvi=5&pl=40&lsparams=ipbypass,mh,mip,mm,mn,ms,mv,mvi,pl&lsig=AM8Gb2swRgIhAJ7g12z_QPjl61sIoulLwnm69IdrmmmcxYLZGg36OoysAiEAzIWDutm5C5_lOEfqw39BrAsLZKNB7_e_0htu6xen5Jw%3D",
                "https://rr1---sn-4g5e6nzl.googlevideo.com/videoplayback?expire=1700796333&ei=TcNfZYWCHO2l0u8P8Pa04Aw&ip=2a01%3A4f9%3A6b%3A4c50%3A%3A2&id=o-APizW-czqRp4AfsI5iVL3OxdknYZpjU81KsUroOmi5Y9&itag=18&source=youtube&requiressl=yes&siu=1&vprv=1&mime=video%2Fmp4&cnr=14&ratebypass=yes&dur=20.062&lmt=1696997740069072&fexp=24007246&c=ANDROID_TESTSUITE&txp=5538434&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Csiu%2Cvprv%2Cmime%2Ccnr%2Cratebypass%2Cdur%2Clmt&sig=ANLwegAwRQIgf_a8yVMcu2noXuBvnTGnekGS0VzsVZx-vbwsblU9ba8CIQDjXugm2bieAdFYTvofUqJFERA3InAuZNQ_74skJtJzTQ%3D%3D&title=y2mate.is%20-%20Do+p%C5%AFlnoci+objedn%C3%A1%C5%A1%2C+r%C3%A1no+v+AlzaBoxu+m%C3%A1%C5%A1+-+Online+-+20s+-+CZ-lrKCOsA-f3Q-360p-1700774736&rm=sn-5golr7l&req_id=92d7044b4d15a3ee&cm2rm=sn-8vq54voxj1-2gbl7l,sn-2gbey7z&ipbypass=yes&redirect_counter=3&cms_redirect=yes&cmsv=e&mh=yM&mip=2a02:8308:295:0:b841:6831:e1af:a697&mm=34&mn=sn-4g5e6nzl&ms=ltu&mt=1700774431&mv=m&mvi=1&pl=40&lsparams=ipbypass,mh,mip,mm,mn,ms,mv,mvi,pl&lsig=AM8Gb2swQwIfWj9wEfoNMppH8T7RMstP3fi2hG0l8cY9-CsTehmL9AIgDIbOIQYfB-oF2xpea0v6w7gsqruxWsmyuAT9OUtg_4g%3D",
                "https://srvcdn5.2convert.me/dl?hash=OlqnVD5ktnrBRKSlYTxAAkD%2B3w42ZB9D8F2e1%2BqS39EwbyzxtEXW4hcam%2BUUG3GCww5SumlV%2BdxrieIdmSy0YIEaqUJ5jQzgMeOl5%2BEq3MOHgioau7kfa7UFBXNbJAqdGikSFHQjmkwkMb%2F8AF0HULWOpjdrT3vj4HEPA2GBlApmLHsIXJEkjyaYLQTIqPHLXXbpYKNYaPWyY3zH2uiqubG0bHvA2fdAYofInwNpgyIhOVbd76TMHqWKXekAZiOFMWi1zWU9eIP1PCuNMPMb5rVk9ceHaEz2Ai6mRsT9amgEGvwnMJ73mnl4QA5LS6Zy",
            };
            gameObject.GetComponent<VideoPlayer>().url = links[Random.Range(0, links.Length)];
            gameObject.GetComponent<VideoPlayer>().loopPointReached += OnVideoEnd;
        }

        void OnVideoEnd(VideoPlayer vp)
        {
            vp.loopPointReached -= OnVideoEnd;
            DOTween.KillAll(true);
            SceneManager.LoadScene("Game");
        }
    }
}
