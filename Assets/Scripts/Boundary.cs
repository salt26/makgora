using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Boundary
{
    // public static float xMin = -1.19f, xMax = 1.19f, yMin = -0.88f, yMax = 0.88f, zMin = -5f, zMax = 5f;
    public static float xMin = -2f, xMax = 2f, yMin = -1.4f, yMax = 1.4f, zMin = -5f, zMax = 5f;
    public static int pageNum = 40, pageBase = 10; // 실제 페이지 수는 pageNum + 1입니다.

    /// <summary>
    /// 페이지 번호를 z좌표로 변환합니다.
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static float PageToZ(int page)
    {
        /*
        if (page < pageBase) return zMin - 1f;
        else if (page > pageBase + pageNum) return zMax + 1f;
        else
        {
            return Mathf.Lerp(zMin, zMax, (page - pageBase) / (float)pageNum);
        }
        */
        return Mathf.LerpUnclamped(zMin, zMax, (page - pageBase) / (float)pageNum);
    }

    /// <summary>
    /// 인자로 주어진 z좌표에서 가장 가까운 페이지 번호를 반환합니다.
    /// </summary>
    /// <param name="z"></param>
    /// <returns></returns>
    public static int ZToPage(float z)
    {
        /*
        if (z < zMin) return pageBase - 1;
        else if (z > zMax) return pageBase + pageNum + 1;
        else
        {
            return Mathf.RoundToInt(Mathf.Lerp(pageBase, pageBase + pageNum, (z - zMin) / (zMax - zMin)));
        }
        */
        return Mathf.RoundToInt(Mathf.LerpUnclamped(pageBase, pageBase + pageNum, (z - zMin) / (zMax - zMin)));
    }

    /// <summary>
    /// 한 페이지를 넘어갈 때의 z좌표의 변화량을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public static float OnePageToDeltaZ()
    {
        return (zMax - zMin) / pageNum;
    }

    /// <summary>
    /// 인자로 주어진 z좌표에서 가장 가까운, 페이지 상의 z좌표를 반환합니다.
    /// 맵 경계를 벗어난 z좌표를 반환할 수도 있습니다.
    /// </summary>
    /// <param name="z"></param>
    /// <returns></returns>
    public static float RoundZ(float z)
    {
        return PageToZ(ZToPage(z));
    } 

    /// <summary>
    /// 인자로 주어진 페이지가 맵에서 갈 수 있는 페이지이면 0을 반환합니다.
    /// 페이지가 너무 작으면 -1을, 너무 크면 1을 반환합니다.
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static int IsValid(int page)
    {
        if (page < pageBase) return -1;
        else if (page > pageBase + pageNum) return 1;
        else return 0;
    }

    /// <summary>
    /// 인자로 주어진 z좌표가 맵 경계 안쪽의 z좌표이면 0을 반환합니다.
    /// z좌표가 너무 작으면 -1을, 너무 크면 1을 반환합니다.
    /// </summary>
    /// <param name="z"></param>
    /// <returns></returns>
    public static int IsValid(float z)
    {
        if (z < zMin) return -1;
        else if (z > zMax) return 1;
        else return 0;
    }
}
