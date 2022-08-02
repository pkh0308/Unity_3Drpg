using System.Collections;

//채집 가능한 아이템용 인터페이스
//아이템  id, 갯수, 채집 시간, sp 소모량의 정보를 담음
public interface ICollectable
{
    int ItemId { get; }
    int ItemCount { get; }
    float SpendTime { get; }
    int SpCount { get; }

    void StartCollect();
    IEnumerator OnCollect();
    void CompleteCollect();
}
