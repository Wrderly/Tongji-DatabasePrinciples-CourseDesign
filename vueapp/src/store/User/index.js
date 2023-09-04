import { initReaderList } from "@/api";

const state = {
  adminName: "",
  readerInfo: {
    readerId: "",
    readerName: "",
    readerPhone: "",
  },
  readerList: [],
  isAdmin: false,
};

const actions = {
  setAdminInfo({ commit }, data) {
    commit("SETADMININFO", data);
  },
  setReaderInfo({ commit }, data) {
    commit("SETREADERINFO", data);
  },
  initReaderList({ commit }) {
    initReaderList().then(
      (res) => {
        console.log(res);
        commit("INITREADERLIST", res.data);
      },
      (err) => console.log(err.message)
    );
  },
};

const mutations = {
  SETADMININFO(state, data) {
    // 保存管理员用户名
    state.adminName = data.userName;
    state.isAdmin = data.isAdmin;
  },
  SETREADERINFO(state, data) {
    // 保存读者用户名
    console.log(data);
    let { readerId, readerName, readerPhone, borrowTimes, ovdTimes, email } =
      data;
    state.readerInfo = {
      readerId,
      readerName,
      readerPhone,
      borrowTimes,
      ovdTimes,
      email,
    };
    state.isAdmin = data.isAdmin;
  },
  INITREADERLIST(state, data) {
    state.readerList = data;
  },
};

const getters = {};

export default {
  state,
  actions,
  mutations,
  getters,
};
