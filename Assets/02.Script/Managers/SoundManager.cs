using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    //MP3 Player -> AudioSource
    //MP3 음원   -> AudioClip 
    //listener   -> AudioListener
    
    //종류별 사운드 오브젝트 객체가 될 오디오 소스
    private AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    //내가 사용한 모든 오디오클립을 저장할 딕셔너리
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    
    /// <summary>
    /// 부모 오브젝트 생성 및 bgm,effect오브젝트 생성 후 부모 오브젝트에 연동
    /// </summary>
    public void Init()
    {
        //부모 오브젝트가 존재하나 체크
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            //부모 오브젝트 생성
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);
            
            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));

            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                //각 bgm, effect이름을 가진 오브젝트 생성
                 GameObject go = new GameObject { name = soundNames[i] };
                 //오브젝트에 오디오소스 컴포넌트 부착
                 _audioSources[i] = go.AddComponent<AudioSource>();
                 //부모 연동
                 go.transform.parent = root.transform;
            }
            //bgm의 경우 무한루프로 지정
            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    public void Clear()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }
    
    /// <summary>
    /// 경로를 지정해 함수를 받아와 실행하는 함수
    /// </summary>
    /// <param name="path">클립의 경로</param>
    /// <param name="type">클립의 타입</param>
    /// <param name="pitch">피치</param>
    public void Play(string path,Define.Sound type = Define.Sound.Effect,  float pitch = 1f)
    {
        //내가 지정한 경로에서 클립을 가져온다
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        //클립을 바탕으로 실행한다.
        Play(audioClip,type,pitch);
    }
    
    /// <summary>
    /// 클립을 직접 넣어 실행하는 함수
    /// </summary>
    /// <param name="audioClip">실행할 클립</param>
    /// <param name="type">클립의 타입</param>
    /// <param name="pitch">클립의 피치</param>
    public void Play(AudioClip audioClip,Define.Sound type = Define.Sound.Effect,  float pitch = 1f)
    {
        //클립이 존재하지 않으면 바로 종료
        if (audioClip == null)
            return;
        
        //클립의 타입이 bgm인 경우
        if (type == Define.Sound.Bgm)
        {
            //bgm오브젝트를 가져옴
            var audioSource = _audioSources[(int)Define.Sound.Bgm];
            //이미 bgm이 실행중이면 일단 중단
            if(audioSource.isPlaying)
                audioSource.Stop();
            
            //클립과 피치를 지정 후 클립 실행
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            //effect오브젝트를 가져옴
            var audioSource = _audioSources[(int)Define.Sound.Effect];
            
            //클립과 피치를 지정 후 클립 실행
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    /// <summary>
    /// 지정된 path의 오디오클립을 꺼내오는 함수
    /// </summary>
    /// <param name="path">꺼내올 클립의 경로</param>
    /// <param name="type">꺼내올 클립의 타입</param>
    /// <returns>꺼내온 클립</returns>
    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        //패스 입력 미스시 강제로 수정
        if (path.Contains("Souns/") == false)
            path = $"Sounds/{path}";
        
        AudioClip audioClip = null;
        
        if (type == Define.Sound.Bgm)
        {
            //지정한 경로에서 클립을 가져온다.
            audioClip = Managers.Resources.Load<AudioClip>(path);
        }
        //효과음의 경우, 종류가 많기 때문에 매번 가져오는 것이 아닌, 한번 사용한 효과음은
        //딕셔너리에 보관 후, 다음에 사용할 땐 딕셔너리에 있는 정보를 가져와 사용한다.
        else
        {
            //아직 한 번도 사용한 적 없는 효과음이면
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                //리소스에서 가져와 사용
                audioClip = Managers.Resources.Load<AudioClip>(path);
                //딕셔너리에 저장
                _audioClips.Add(path,audioClip);
            }
        }
        
        if (audioClip == null)
            Debug.Log($"Audio Clip Missing! {path}");
        
        return audioClip;
    }
}
