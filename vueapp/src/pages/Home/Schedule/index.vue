<template>
    <div id="box">
        <div id="box_p">
            <p>排班表</p>
        </div>
        <table class="mt">
            <thead>
                <tr>
                    <td>编号</td>
                    <td>姓名</td>
                    <td>性别</td>
                    <td>时间</td>
                    <td>区域</td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>001</td>
                    <td>张伟</td>
                    <td>男</td>
                    <td>2023/7/9</td>
                    <td>D</td>
                </tr>
                <tr>
                    <td>002</td>
                    <td>王丽</td>
                    <td>女</td>
                    <td>2023/7/10</td>
                    <td>A</td>
                </tr>
                <tr>
                    <td>003</td>
                    <td>张小明</td>
                    <td>男</td>
                    <td>2023/7/11</td>
                    <td>A</td>
                </tr>
                <tr>
                    <td>004</td>
                    <td>李冰</td>
                    <td>女</td>
                    <td>2023/7/12</td>
                    <td>C</td>
                </tr>
                <tr>
                    <td>005</td>
                    <td>林云</td>
                    <td>女</td>
                    <td>2023/7/13</td>
                    <td>B</td>
                </tr>
                <tr>
                    <td>006</td>
                    <td>王刚</td>
                    <td>男</td>
                    <td>2023/7/14</td>
                    <td>B</td>
                </tr>
                <tr>
                    <td>007</td>
                    <td>王强</td>
                    <td>男</td>
                    <td>2023/7/15</td>
                    <td>D</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>


<style>
    * {
        padding: 0px;
        margin: 0px;
        font-family: 微软雅黑;
    }

    #box {
        border: 1px solid #bbb;
        margin: 50px auto;
        width: 620px;
    }

    #box_p {
        width: 400px;
        position: relative;
        left: 50%;
        margin-left: -200px;
    }

        #box_p p {
            text-align: center;
            padding: 10px 0px 0px 0px;
            font-size: 25px;
            font-weight: bold;
            color: #999;
        }

    table.mt {
        width: 580px;
        margin: 15px 20px 20px 20px;
        border-top: 1px solid #EFD3D2;
        border-spacing: 0px;
        color: #B06A68;
    }

        table.mt thead tr td,
        table.mt tbody tr td {
            border-bottom: 1px solid #EFD3D2;
            padding: 10px 20px;
        }

        table.mt thead tr td {
            font-weight: bold;
        }
</style>