import { initReaderList } from "@/api";

const state = {
    adminInfo: {
        admin_id: "",
        admin_name: "",
        phone_number: "",
        email: "",
    },
  readerInfo: {
      reader_id: "",
      reader_name: "",
      phone_number: "",
      email: "",
      overdue_times:"",
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
        commit("INITREADERLIST", res.readers);
      },
      (err) => console.log(err.message)
    );
  },
};

const mutations = {
  SETADMININFO(state, data) {
    // 保存管理员用户名
        state.adminInfo.admin_id = data.admin_id;
        state.adminInfo.admin_name = data.admin_name;
        state.adminInfo.phone_number = data.phone_number;
        state.adminInfo.email = data.email;
        state.isAdmin = data.isAdmin;
  },
  SETREADERINFO(state, data) {
    // 保存读者用户名
      console.log(data);
      state.readerInfo.reader_id = data.reader_id;
      state.readerInfo.reader_name = data.reader_name;
      state.readerInfo.phone_number = data.phone_number;
      state.readerInfo.email = data.email;
      state.readerInfo.overdue_times = data.overdue_times;
      state.isAdmin = data.isAdmin;
  },
    INITREADERLIST(state, data) {
        data = data || [];
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
