<template>
  <div class="container">
    <div class="lunbo" @mouseenter="clear" @mouseleave="run">
      <div class="img">
        <img :src="dataList[currentIndex]" alt="" />
      </div>
      <div class="dooted" v-if="this.dataList.length">
        <ul class="doo">
          <li
            v-for="(item, index) in this.dataList"
            :key="index"
            :class="{ current: currentIndex == index }"
            @click="gotoPage(index)"
          ></li>
        </ul>
      </div>
      <div class="right turn" @click="next()">
        <i class="el-icon-arrow-right"></i>
      </div>
      <div class="left turn" @click="up()">
        <i class="el-icon-arrow-left"></i>
      </div>
    </div>
    <div class="intro">
      <h2>借书规则</h2>
      <p>
        续借次数不得超过5次，并请及时归还书籍。
      </p>
    </div>
  </div>
</template>

<script>
export default {
  name: "Userhome",
  data() {
    return {
      dataList: [
        "./images/user1.jpg",
        "./images/user2.jpg",
        "./images/user3.jpg",
      ],
      currentIndex: 0, // 默认显示图片
      timer: null, // 定时器
    };
  },
  created() {
    this.run();
  },
  methods: {
    gotoPage(index) {
      this.currentIndex = index;
    },
    next() {
      if (this.currentIndex === this.dataList.length - 1) {
        this.currentIndex = 0;
      } else {
        this.currentIndex++;
      }
    },
    up() {
      if (this.currentIndex === 0) {
        this.currentIndex = this.dataList.length - 1;
      } else {
        this.currentIndex--;
      }
    },
    clear() {
      clearInterval(this.timer);
    },
    // 定时器
    run() {
      this.timer = setInterval(() => {
        this.next();
      }, 2000);
    },
  },
};
</script>

<style lang="less" scoped>
ul li {
  float: left;
  width: 30px;
  height: 40px;
  line-height: 40px;
  text-align: center;
  cursor: pointer;
  color: rgba(240, 238, 238, 0.8);
  font-size: 14px;
}
.container {
  position: relative;
  height: 400px;
  width: 802px;
  margin-left: 60px;
  .img {
    height: 400px;
    width: 800px;
    border: 1px solid gray;
    img {
      height: 100%;
      width: 100%;
    }
  }
  .dooted {
    position: absolute;
    bottom: -10px;
    right: 0px;
  }
}
.turn {
  width: 20px;
  height: 20px;
  line-height: 20px;
  border-radius: 5px;
  cursor: pointer;
  background-color: #d0d0d073;
}
.right {
  position: absolute;
  top: 200px;
  right: 0;
}
.left {
  position: absolute;
  top: 200px;
  left: 0;
}
.current {
  color: gray;
}
</style>
