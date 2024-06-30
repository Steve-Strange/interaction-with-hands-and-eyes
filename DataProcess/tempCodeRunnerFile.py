    # 创建颜色条
    legend_elements = [plt.Rectangle((0,0),1,1, color=c, label=et) for c, et in zip(colors, event_typeToColor)]