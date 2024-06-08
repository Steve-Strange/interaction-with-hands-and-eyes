import math
import pandas as pd
import scipy.stats as stats


def average(attr, method):  # 使用方法为（ours、bubble、射线）的数据平均值
    """

    :param attr:
    :param method:
    :return:
    """
    file_path = "output.xlsx"
    df = pd.read_excel(file_path, sheet_name='Sheet1')
    usefulData = df[df["使用方法"] == method][attr]
    number = 0
    sum_ = 0
    for i in usefulData:
        print(i)
        sum_ += i
        number += 1
    return sum_ / number


def biaozhuncha(attr, method):  # 使用方法为（ours、bubble、射线）的数据标准差
    """

    :param attr:
    :param method:
    :return:
    """
    average_ = average(attr, method)
    file_path = "output.xlsx"
    df = pd.read_excel(file_path, sheet_name='Sheet1')
    usefulData = df[df["使用方法"] == method][attr]
    number = 0
    sum_ = 0
    for i in usefulData:
        sum_ += (average_ - i) * (average_ - i)
        number += 1
    return math.sqrt(sum_ / number)


def t_test(method1, method2, attr):  # 两个方法在attr属性上的p值
    """

    :param method1:
    :param method2:
    :param attr:
    """
    file_path = "output.xlsx"
    df = pd.read_excel(file_path, sheet_name='Sheet1')
    usefulData1 = df[df["使用方法"] == method1][attr]
    usefulData2 = df[df["使用方法"] == method2][attr]
    group1 = []
    group2 = []
    for i in usefulData1:
        group1.append(i)
    for i in usefulData2:
        group2.append(i)
    # 进行独立样本t检验
    t_stat, p_value = stats.ttest_ind(group1, group2)
    print(f"t统计量: {t_stat}")
    print(f"p值: {p_value}")


def cohen(m_1, m_2, sd_1, sd_2):
    """

    :param m_1:
    :param m_2:
    :param sd_1:
    :param sd_2:
    :return:
    """
    return (m_1 - m_2) / math.sqrt((sd_1 * sd_1 - sd_2 * sd_2) / 2)


average("precision", "ours")
