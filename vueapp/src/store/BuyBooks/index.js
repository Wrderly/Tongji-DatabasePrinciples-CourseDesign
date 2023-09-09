import { initBuyBookList } from "@/api";

const state = {
    buybookList: [],
};

const actions = {
    initBuyBookList({ commit }, data) {
        let Bookobj = {
            supplier_id: data
        };
        initBuyBookList(Bookobj).then(
            (res) => {
                console.log(res);
                commit("INITBUYBOOKLIST", res.purchaseRecords);
            },
            (err) => console.log(err.message)
        );
    },
};

const mutations = {
    INITBUYBOOKLIST(state, data) {
        data = data || [];
        state.buybookList = data;
    },
};

const getters = {};

export default {
    state,
    actions,
    mutations,
    getters,
};