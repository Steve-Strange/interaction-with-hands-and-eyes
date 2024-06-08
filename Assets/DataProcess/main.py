# data_processing
import os
import pandas as pd

# 指定文件夹路径
folder_path = 'select_ours'

# 存储所有txt文件路径的列表
txt_files = [os.path.join(folder_path, f) for f in os.listdir(folder_path) if f.endswith('.txt')]
print(txt_files)
# 创建一个空的DataFrame，用于存储处理结果
data = []
headers = ["样本序号", "precision", "左手移动距离", "右手移动距离", "头移动距离", "左手移动角度", "右手移动角度",
             "	头移动角度", "花费时间", "使用方法"]
# 遍历所有txt文件
number = 0
for txt_file in txt_files:
    temp = []
    temp.append(number)
    with open(txt_file, 'r', encoding='utf-8') as file:
        # 读取文件内容
        for line in file:
            if line[0:9] == "precision":
                temp.append(float(line[10:]))
            if line[0:19] == "Moved Distance head":
                temp.append(float(line[20:]))
            if line[0:20] == "Moved Distance right":
                temp.append(float(line[21:]))
            if line[0:19] == "Moved Distance left":
                temp.append(float(line[20:]))
            if line[0:18] == "Rotated Angle head":
                temp.append(float(line[19:]))
            if line[0:19] == "Rotated Angle right":
                temp.append(float(line[20:]))
            if line[0:18] == "Rotated Angle left":
                temp.append(float(line[19:]))
            if line[0:18] == "all selection time":
                temp.append(float(line[19:]))
    temp.append(folder_path)
            # 假设处理是将文件名和内容存储起来
    data.append(temp)
    number += 1
# 将数据转换为DataFrame
df = pd.DataFrame(data, columns=headers)

# 指定输出的Excel文件路径
excel_file = 'output.xlsx'

# 将DataFrame写入Excel文件
df.to_excel(excel_file, index=False)

print(f"处理完成，结果已保存到 {excel_file}")
