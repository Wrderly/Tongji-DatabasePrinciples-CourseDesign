import { initBooktypeList } from "@/api";

const state = {
    booktypeList: [],
};

const actions = {
    initBooktypeList({ commit }) {
        initBooktypeList().then(
            (res) => {
                console.log(res);
                commit("INITBOOKTYPELIST", res.typeLists);
            },
            (err) => console.log(err.message)
        );
    },
};

const mutations = {
    INITBOOKTYPELIST(state, data) {
        data = data || [];
        state.booktypeList = data;
    },
};

const getters = {};

export default {
    state,
    actions,
    mutations,
    getters,
};